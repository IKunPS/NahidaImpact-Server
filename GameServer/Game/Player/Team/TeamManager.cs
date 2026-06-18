using NahidaImpact.Database;
using NahidaImpact.Database.Avatar;
using NahidaImpact.Database.Team;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.GameServer.Game.Worlds;
using NahidaImpact.GameServer.Server.Packet.Send.Team;
using NahidaImpact.GameServer.Server.Packet.Send.Scene;
using NahidaImpact.GameServer.Server.Packet.Send.Avatar;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;
using NahidaImpact.Util;

namespace NahidaImpact.GameServer.Game.Player.Team;

public class TeamManager : BasePlayerManager
{

    private readonly TeamData _teamData;

    private readonly SortedDictionary<int, TeamInfo> _teams = [];
    public Dictionary<int, TeamInfo> Teams => new(_teams);

    private readonly List<EntityAvatar> _activeTeam = [];

    public EntityTeam? Entity { get; set; }

    public void SetEntity(EntityTeam entity) => Entity = entity;

    public TeamInfo MpTeam { get; set; } = new();

    public bool UsingTrialTeam { get; set; }
    public TeamInfo TrialAvatarTeam { get; set; } = new();
    
    public HashSet<string> TeamAbilityEmbryos { get; set; } = [];
    public Dictionary<int, AvatarDataInfo> TrialAvatars { get; set; } = [];

    public int CurrentTeamIndex { get; set; } = 1;
    public int CurrentCharacterIndex { get; set; }

    public TeamManager(PlayerInstance player) : base(player)
    {
        _teamData = DatabaseHelper.GetInstanceOrCreateNew<TeamData>(player.Uid);

        // Load teams from DB
        foreach (var team in _teamData.Teams)
        {
            _teams[team.Index] = team;
        }

        // Ensure default teams 1..DEFAULT_TEAMS exist
        for (int i = 1; i <= GameConstants.DEFAULT_TEAMS; i++)
        {
            if (!_teams.ContainsKey(i))
            {
                _teams[i] = new TeamInfo { Index =i };
            }
        }
    }

    public void Save()
    {
        _teamData.Teams = [.. _teams.Values];
        TeamData.SaveTeamData(_teamData);
    }

    public TeamInfo GetCurrentTeamInfo()
    {
        if (UsingTrialTeam && TrialAvatarTeam.AvatarGuidList.Count > 0)
            return TrialAvatarTeam;
        return _teams.GetValueOrDefault(CurrentTeamIndex, new TeamInfo { Index =CurrentTeamIndex });
    }

    public TeamInfo GetCurrentSinglePlayerTeamInfo()
        => _teams.GetValueOrDefault(CurrentTeamIndex, new TeamInfo { Index =CurrentTeamIndex });

    public List<EntityAvatar> GetActiveTeam(bool fix = false)
    {
        if (!fix) return _activeTeam;

        // Deduplicate active team
        var seen = new HashSet<ulong>();
        _activeTeam.RemoveAll(e =>
        {
            if (seen.Contains(e.AvatarInfo.Guid)) return true;
            seen.Add(e.AvatarInfo.Guid);
            return false;
        });
        return _activeTeam;
    }

    public EntityAvatar? GetCurrentAvatarEntity()
    {
        if (_activeTeam.Count == 0) return null;
        if (CurrentCharacterIndex >= _activeTeam.Count)
            CurrentCharacterIndex = 0;
        return _activeTeam[CurrentCharacterIndex];
    }

    public ulong GetCurrentCharacterGuid()
        => GetCurrentAvatarEntity()?.AvatarInfo.Guid ?? 0;

    public int GetCurrentCharacterIndex() => CurrentCharacterIndex;
    public void SetCurrentCharacterIndex(int index) => CurrentCharacterIndex = Math.Max(0, index);

    public bool IsUsingTrialTeam() => UsingTrialTeam;

    public int GetMaxTeamSize()
        => GameConstants.MAX_AVATARS_IN_TEAM;

    public bool CanAddAvatarToCurrentTeam()
        => GetCurrentTeamInfo().AvatarGuidList.Count < GetMaxTeamSize();
    
    public void SetCurrentTeam(int teamId)
    {
        if (!_teams.ContainsKey(teamId)) return;
        CurrentTeamIndex = teamId;
        _ = UpdateTeamEntitiesAsync();
        // TODO: Send PacketChooseCurAvatarTeamRsp(teamId)
    }

    public void SetTeamName(int teamId, string teamName)
    {
        if (!_teams.TryGetValue(teamId, out var team)) return;
        team.Name = teamName;
        Save();
        // TODO: Send PacketChangeTeamNameRsp(teamId, teamName)
    }

    public void SetupAvatarTeam(int teamId, List<ulong> guidList)
    {
        if (guidList.Count == 0 || guidList.Count > GetMaxTeamSize())
            return;

        if (!_teams.TryGetValue(teamId, out var team))
            return;

        // Validate all GUIDs belong to player
        var validGuids = new List<ulong>();
        foreach (var guid in guidList)
        {
            var avatar = Player.AvatarManager.GetAvatarByGuid(guid);
            if (avatar == null) return;
            if (validGuids.Contains(guid)) return;
            validGuids.Add(guid);
        }

        team.AvatarGuidList = validGuids;
        Save();

        // Send team update notify to client
        _ = Player.SendPacket(new PacketAvatarTeamUpdateNotify(Player));

        // If updating current team, rebuild entities and send response
        if (teamId == CurrentTeamIndex)
            _ = UpdateTeamEntitiesAsync(new PacketSetUpAvatarTeamRsp(Player, teamId, team));
        else
            _ = Player.SendPacket(new PacketSetUpAvatarTeamRsp(Player, teamId, team));
    }

    public void SetupMpTeam(List<ulong> guidList)
    {
        if (guidList.Count == 0 || guidList.Count > GetMaxTeamSize())
            return;

        var validGuids = new List<ulong>();
        foreach (var guid in guidList)
        {
            var avatar = Player.AvatarManager.GetAvatarByGuid(guid);
            if (avatar == null) return;
            if (validGuids.Contains(guid)) return;
            validGuids.Add(guid);
        }

        MpTeam.AvatarGuidList = validGuids;
        // TODO: MP team update packet
    }

    public void AddNewCustomTeam()
    {
        if (_teams.Count >= GameConstants.MAX_TEAMS) return;

        // Find lowest available ID starting from 5
        int id = -1;
        for (int i = 5; i <= GameConstants.MAX_TEAMS; i++)
        {
            if (!_teams.ContainsKey(i))
            {
                id = i;
                break;
            }
        }
        if (id < 0) return;

        _teams[id] = new TeamInfo { Index =id };
        Save();
        // TODO: Send PacketAvatarTeamAllDataNotify, PacketAddBackupAvatarTeamRsp
    }

    public void RemoveCustomTeam(int id)
    {
        if (id <= GameConstants.DEFAULT_TEAMS || !_teams.ContainsKey(id)) return;
        _teams.Remove(id);
        Save();
        // TODO: Send PacketAvatarTeamAllDataNotify, PacketDelBackupAvatarTeamRsp
    }

    public async ValueTask UpdateTeamEntitiesAsync(BasePacket? responsePacket = null)
    {
        var teamInfo = GetCurrentTeamInfo();
        if (teamInfo.AvatarGuidList.Count == 0) return;

        var currentEntity = GetCurrentAvatarEntity();
        var existing = new Dictionary<ulong, EntityAvatar>();
        int prevSelectedIndex = -1;

        foreach (var entity in _activeTeam)
            existing[entity.AvatarInfo.Guid] = entity;

        _activeTeam.Clear();

        for (int i = 0; i < teamInfo.AvatarGuidList.Count; i++)
        {
            var guid = teamInfo.AvatarGuidList[i];
            EntityAvatar? entity;
            if (existing.TryGetValue(guid, out var existingEntity))
            {
                entity = existingEntity;
                existing.Remove(guid);
                if (entity == currentEntity)
                    prevSelectedIndex = i;
            }
            else
            {
                var avatarInfo = Player.AvatarManager.GetAvatarByGuid(guid);
                if (avatarInfo == null) continue;
                entity = EntityCreationEvent.Call<EntityAvatar>(
                    [typeof(Scene), typeof(AvatarDataInfo)],
                    [Player.Scene, avatarInfo]);
                if (entity == null) continue;
            }
            _activeTeam.Add(entity);
        }

        // Remove old entities from scene
        foreach (var removed in existing.Values)
        {
            Player.Scene?.RemoveEntity(removed);
        }

        if (prevSelectedIndex < 0)
            prevSelectedIndex = Math.Min(CurrentCharacterIndex, _activeTeam.Count - 1);
        CurrentCharacterIndex = prevSelectedIndex;

        if (responsePacket != null)
            await Player.SendPacket(responsePacket);

        await Task.CompletedTask;
    }

    public async ValueTask UpdateTeamEntitiesAsync()
        => await UpdateTeamEntitiesAsync(null);

    public void ChangeAvatar(ulong guid)
    {
        var oldEntity = GetCurrentAvatarEntity();
        if (oldEntity == null || guid == oldEntity.AvatarInfo.Guid) return;

        EntityAvatar? newEntity = null;
        int index = -1;
        for (int i = 0; i < _activeTeam.Count; i++)
        {
            if (guid == _activeTeam[i].AvatarInfo.Guid)
            {
                index = i;
                newEntity = _activeTeam[i];
                break;
            }
        }

        if (index < 0 || newEntity == oldEntity) return;

        CurrentCharacterIndex = index;
        Player.Scene?.ReplaceEntity(oldEntity, newEntity);
        _ = Player.SendPacket(new PacketChangeAvatarRsp(guid));
    }

    public void AddAvatarToCurrentTeam(ulong guid)
    {
        var teamInfo = GetCurrentTeamInfo();
        if (teamInfo.AvatarGuidList.Count >= GetMaxTeamSize()) return;
        if (teamInfo.AvatarGuidList.Contains(guid)) return;
        teamInfo.AvatarGuidList.Add(guid);
        Save();

        _ = Player.SendPacket(new PacketAvatarTeamUpdateNotify(Player));
        _ = UpdateTeamEntitiesAsync();
    }

    public void OnAvatarDie(ulong dieGuid)
    {
        var deadEntity = GetCurrentAvatarEntity();
        if (deadEntity == null || deadEntity.Id != (uint)dieGuid) return;

        // Find replacement
        int replaceIndex = -1;
        for (int i = 0; i < _activeTeam.Count; i++)
        {
            if (_activeTeam[i].IsAlive() && i != CurrentCharacterIndex)
            {
                replaceIndex = i;
                break;
            }
        }

        if (replaceIndex >= 0)
        {
            CurrentCharacterIndex = replaceIndex;
            Player.Scene?.AddEntity(_activeTeam[replaceIndex]);
        }
        // TODO: Send death notify, handle team wipe

        // TODO: Send PacketAvatarDieAnimationEndRsp
    }

    public AbilityControlBlock GetAbilityControlBlock()
    {
        var block = new AbilityControlBlock();
        int embryoId = 0;

        // 1. Default team abilities
        foreach (var skill in GameConstants.DEFAULT_TEAM_ABILITY_STRINGS)
        {
            block.AbilityEmbryoList.Add(new AbilityEmbryo
            {
                AbilityId = (uint)(++embryoId),
                AbilityNameHash = Utils.AbilityHash(skill),
                AbilityOverrideNameHash = GameConstants.DEFAULT_ABILITY_NAME
            });
        }

        // 2. Team ability embryos (from team resonances, etc.)
        foreach (var embryo in TeamAbilityEmbryos)
        {
            block.AbilityEmbryoList.Add(new AbilityEmbryo
            {
                AbilityId = (uint)(++embryoId),
                AbilityNameHash = Utils.AbilityHash(embryo),
                AbilityOverrideNameHash = GameConstants.DEFAULT_ABILITY_NAME
            });
        }

        // 3. Scene LevelEntity team abilities
        var scene = Player.Scene;
        if (scene != null)
        {
            var levelEntityConfig = scene.SceneData?.LevelEntityConfig;
            if (!string.IsNullOrEmpty(levelEntityConfig)
                && Data.GameData.ConfigLevelEntityDataMap.TryGetValue(levelEntityConfig, out var config))
            {
                if (config.TeamAbilities != null)
                {
                    foreach (var abilityData in config.TeamAbilities)
                    {
                        if (!string.IsNullOrEmpty(abilityData.AbilityName))
                        {
                            block.AbilityEmbryoList.Add(new AbilityEmbryo
                            {
                                AbilityId = (uint)(++embryoId),
                                AbilityNameHash = Utils.AbilityHash(abilityData.AbilityName),
                                AbilityOverrideNameHash = GameConstants.DEFAULT_ABILITY_NAME
                            });
                        }
                    }
                }
            }
        }

        return block;
    }
}
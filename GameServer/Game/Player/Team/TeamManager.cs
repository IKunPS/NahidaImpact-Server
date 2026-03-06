using NahidaImpact.Database;
using NahidaImpact.Database.Team;
using NahidaImpact.Database.Avatar;
using NahidaImpact.GameServer.Game.Avatar;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.GameServer.Game.World;
using NahidaImpact.GameServer.Game.Player.Team;
using NahidaImpact.GameServer.Server.Packet.Send.Avatar;
using NahidaImpact.GameServer.Server.Packet.Send.Scene;
using NahidaImpact.GameServer.Server.Packet;
using NahidaImpact.Proto;
using System.Collections.Generic;
using System.Linq;
using NahidaImpact.Util;
using NahidaImpact.KcpSharp;
using System;
using System.Linq;

namespace NahidaImpact.GameServer.Game.Player.Team;

public class TeamManager : BasePlayerManager
{
    public static Logger Logger { get; } = new("TeamManager");
    private TeamData _teamData;
    private List<EntityAvatar> _activeTeamAvatars;
    private HashSet<int> _teamResonances;
    private HashSet<int> _teamResonancesConfig;
    private HashSet<string> _teamAbilityEmbryos;
    private TeamInfo _mpTeam;
    private int _useTemporarilyTeamIndex = -1;
    private List<TeamInfo> _temporaryTeams;
    private bool _usingTrialTeam;
    private TeamInfo? _trialAvatarTeam;
    private Dictionary<int, uint> _trialAvatars; // avatarId -> avatarGuid mapping
    private int _previousIndex = -1;
    
    private EntityTeam? _entity;

    public TeamManager(PlayerInstance player) : base(player)
    {
        _teamData = DatabaseHelper.GetInstanceOrCreateNew<TeamData>(player.Uid);
        _activeTeamAvatars = new List<EntityAvatar>();
        _teamResonances = new HashSet<int>();
        _teamResonancesConfig = new HashSet<int>();
        _teamAbilityEmbryos = new HashSet<string>();
        _trialAvatars = new Dictionary<int, uint>();
        _mpTeam = new TeamInfo();
        _temporaryTeams = [];

        InitializeTeams();
    }

    private void InitializeTeams()
    {
        if (_teamData.Teams.Count == 0)
        {
            // Create default teams
            for (uint i = 1; i <= GameConstants.DEFAULT_TEAMS; i++)
            {
                _teamData.Teams.Add(new TeamInfo { Index = i });
            }
            _teamData.CurrentTeamIndex = 1;
            Save();
        }

        // Load mp team
        if (_teamData.MpTeam != null)
        {
            _mpTeam = _teamData.MpTeam;
        }
        else
        {
            _mpTeam = new TeamInfo();
            _teamData.MpTeam = _mpTeam;
            try
            {
                Save();
            }
            catch (Exception ex) when (ex.Message.Contains("NOT NULL constraint failed"))
            {
                // If update fails due to NOT NULL constraint, delete and recreate the record
                DatabaseHelper.DeleteInstance<TeamData>(_teamData.Uid);
                DatabaseHelper.CreateInstance(_teamData);
            }
        }

        // Load temporary teams
        if (_teamData.TemporaryTeams != null)
        {
            _temporaryTeams = _teamData.TemporaryTeams;
        }
        else
        {
            _temporaryTeams = [];
            _teamData.TemporaryTeams = _temporaryTeams;
            Save();
        }

        // Load trial team data
        _usingTrialTeam = _teamData.UsingTrialTeam;
        _trialAvatarTeam = _teamData.TrialAvatarTeam;
        
        if (_teamData.TrialAvatars != null)
        {
            _trialAvatars = _teamData.TrialAvatars;
        }
        else
        {
            _trialAvatars = [];
            _teamData.TrialAvatars = _trialAvatars;
            Save();
        }
        
        _previousIndex = _teamData.PreviousIndex;
        _useTemporarilyTeamIndex = _teamData.UseTemporarilyTeamIndex;
    }

    public TeamData TeamData => _teamData;

    public List<TeamInfo> Teams => _teamData.Teams;

    public List<GameAvatarTeam> GetAvatarTeams()
    {
        return Teams.Select(t => new GameAvatarTeam
        {
            Index = t.Index,
            Name = t.Name,
            AvatarGuidList = t.AvatarGuidList
        }).ToList();
    }

    public uint CurrentTeamIndex => _teamData.CurrentTeamIndex;

    private TeamInfo GetCurrentTeamInfoCore()
    {
        if (_useTemporarilyTeamIndex >= 0 && _useTemporarilyTeamIndex < _temporaryTeams.Count)
        {
            return _temporaryTeams[_useTemporarilyTeamIndex];
        }
        if (Player.IsInMultiplayer())
        {
            return _mpTeam;
        }
        return Teams.FirstOrDefault(t => t.Index == CurrentTeamIndex) ?? Teams[0];
    }

    public GameAvatarTeam GetCurrentTeamInfo()
    {
        var teamInfo = GetCurrentTeamInfoCore();
        return new GameAvatarTeam
        {
            Index = teamInfo.Index,
            Name = teamInfo.Name,
            AvatarGuidList = teamInfo.AvatarGuidList
        };
    }

    internal TeamInfo GetCurrentTeamInfoInternal()
    {
        // Check temporary team and multiplayer team conditions first (same as GetCurrentTeamInfoCore)
        // But we need to log which type of team we're returning
        if (_useTemporarilyTeamIndex >= 0 && _useTemporarilyTeamIndex < _temporaryTeams.Count)
        {

            return _temporaryTeams[_useTemporarilyTeamIndex];
        }
        if (Player.IsInMultiplayer())
        {

            return _mpTeam;
        }
        
        // Special case: Teams is empty - create default team
        if (Teams.Count == 0)
        {

            var defaultTeam = new TeamInfo { Index = CurrentTeamIndex };
            _teamData.Teams.Add(defaultTeam);
            Save();

            return defaultTeam;
        }
        
        // Use the core logic for normal team selection
        var teamInfo = GetCurrentTeamInfoCore();

        return teamInfo;
    }

    public List<EntityAvatar> GetActiveTeam()
    {
        return _activeTeamAvatars;
    }

    public EntityAvatar? GetCurrentAvatarEntity()
    {
        if (_activeTeamAvatars.Count == 0)
            return null;

        if (_previousIndex < 0 || _previousIndex >= _activeTeamAvatars.Count)
            _previousIndex = 0;

        return _activeTeamAvatars[_previousIndex];
    }

    public int GetMaxTeamSize()
    {
        // TODO: Consider multiplayer limits
        return GameConstants.MAX_AVATARS_IN_TEAM;
    }

    public bool CanAddAvatarToTeam(GameAvatarTeam team)
    {
        return team.Size < GetMaxTeamSize();
    }

    public bool AddAvatarToTeam(GameAvatarTeam team, ulong avatarGuid)
    {
        if (!CanAddAvatarToTeam(team))
            return false;

        if (team.Contains(avatarGuid))
            return false;

        team.AddAvatar(avatarGuid);
        return true;
    }

    public bool RemoveAvatarFromTeam(GameAvatarTeam team, int slot)
    {
        return team.RemoveAvatar(slot);
    }

    public void SetCurrentTeam(uint teamIndex)
    {
        if (teamIndex < 1 || teamIndex > Teams.Count)
            return;

        _teamData.CurrentTeamIndex = teamIndex;
        Save();
    }

    public async void SetTeamName(uint teamIndex, string name)
    {
        var team = Teams.FirstOrDefault(t => t.Index == teamIndex);
        if (team == null)
            return;

        team.Name = name;
        Save();
        // Send packet
        await Player.SendPacket(new PacketChangeTeamNameRsp((int)teamIndex, name));
    }

    public bool AddAvatarToCurrentTeam(ulong avatarGuid)
    {

        var currentTeam = GetCurrentTeamInfoInternal();
        if (currentTeam == null)
        {
            return false;
        }

        if (currentTeam.Contains(avatarGuid))
        {

            return false;
        }
        if (currentTeam.Size >= GetMaxTeamSize())
        {

            return false;
        }

        currentTeam.AddAvatar(avatarGuid);
        Save();

        return true;
    }

    private async void UpdateTeamProperties()
    {
        if (Player.EntityAvatar == null)
            return;
        
        UpdateTeamResonances();
        await Player.SendPacket(new PacketSceneTeamUpdateNotify(Player));
    }

    private void UpdateTeamResonances()
    {
        // TODO: Implement team resonance logic
        _teamResonances.Clear();
        _teamResonancesConfig.Clear();
    }

    public EntityTeam? Entity
    {
        get => _entity;
        set => _entity = value;
    }
    
    public void SetEntity(EntityTeam entity)
    {
        _entity = entity;
    }

    private void Save()
    {
        // Ensure fields are not null before saving
        if (_mpTeam == null)
            _mpTeam = new TeamInfo();
        if (_temporaryTeams == null)
            _temporaryTeams = [];
        if (_trialAvatars == null)
            _trialAvatars = [];

        // Update TeamData fields
        _teamData.MpTeam = _mpTeam;
        _teamData.UsingTrialTeam = _usingTrialTeam;
        _teamData.TrialAvatarTeam = _trialAvatarTeam;
        _teamData.TrialAvatars = _trialAvatars;
        _teamData.PreviousIndex = _previousIndex;
        _teamData.UseTemporarilyTeamIndex = _useTemporarilyTeamIndex;
        _teamData.TemporaryTeams = _temporaryTeams;

        DatabaseHelper.UpdateInstance(_teamData);
    }

    public TeamInfo GetMpTeam() => _mpTeam;
    
    public TeamInfo GetCurrentSinglePlayerTeamInfo()
    {
        // Returns the current single player team (not multiplayer team)
        if (_useTemporarilyTeamIndex >= 0 && _useTemporarilyTeamIndex < _temporaryTeams.Count)
        {
            return _temporaryTeams[_useTemporarilyTeamIndex];
        }
        return Teams.FirstOrDefault(t => t.Index == CurrentTeamIndex) ?? Teams[0];
    }
    
    /// <summary>
    /// Gets the current character index in the active team.
    /// </summary>
    public int GetCurrentCharacterIndex()
    {
        return _previousIndex;
    }
    
    public void SetCurrentCharacterIndex(int index)
    {
        if (index >= 0 && index < _activeTeamAvatars.Count)
        {
            _previousIndex = index;
            Player.EntityAvatar = GetCurrentAvatarEntity();
        }
    }
    
    /// <summary>
    /// Updates team entities based on current team configuration.
    /// Reuses existing entities where possible, removes old ones, and creates new ones.
    /// </summary>
    /// <param name="responsePacket">Optional response packet to send after update.</param>
    public void UpdateTeamEntities(BasePacket? responsePacket = null)
    {
        // Sanity check - Should never happen
        var currentTeamInfo = GetCurrentTeamInfoInternal();
        if (currentTeamInfo.AvatarGuidList.Count <= 0)
        {
            return;
        }

        // If current team has changed
        var currentEntity = GetCurrentAvatarEntity();
        var existingAvatars = new Dictionary<int, EntityAvatar>();
        var prevSelectedAvatarIndex = -1;

        foreach (var entity in _activeTeamAvatars)
        {
            existingAvatars[(int)entity.AvatarInfo.AvatarId] = entity;
        }

        // Clear active team entity list
        _activeTeamAvatars.Clear();

        // Add back entities into team
        for (int i = 0; i < currentTeamInfo.AvatarGuidList.Count; i++)
        {
            var avatarGuid = currentTeamInfo.AvatarGuidList[i];
            EntityAvatar entity;
            
            // Get avatar by guid
            var avatar = Player.Avatars.FirstOrDefault(a => a.Guid == avatarGuid);
            if (avatar == null)
            {
                Logger.Warn($"Avatar with guid {avatarGuid} not found for player {Player.Uid}");
                continue;
            }
            
            var avatarId = avatar.AvatarId;
            if (existingAvatars.ContainsKey((int)avatarId))
            {
                entity = existingAvatars[(int)avatarId];
                existingAvatars.Remove((int)avatarId);
                if (entity == currentEntity)
                {
                    prevSelectedAvatarIndex = i;
                }
            }
            else
            {
                // Create new entity
                entity = EntityCreationEvent.Call<EntityAvatar>(
                    new Type[] { typeof(PlayerInstance), typeof(AvatarDataInfo) },
                    new object[] { Player, avatar });
                
                if (entity == null)
                {
                    Logger.Warn($"Failed to create entity for avatar {avatarId} for player {Player.Uid}");
                    continue;
                }
            }

            _activeTeamAvatars.Add(entity);
        }

        // Unload removed entities
        foreach (var entity in existingAvatars.Values)
        {
            Player.Scene?.RemoveEntity(entity);
            // TODO: Implement avatar save
            // entity.AvatarInfo.Save();
        }

        // If no avatars were added, reset previous index and return
        if (_activeTeamAvatars.Count == 0)
        {
            _previousIndex = -1;
            UpdateTeamProperties();
            return;
        }

        // Set new selected character index
        if (prevSelectedAvatarIndex == -1)
        {
            // Previous selected avatar is not in the same spot, we will select the current one in the
            // prev slot
            // Clamp previous index to valid range
            int clampedIndex = _previousIndex;
            if (clampedIndex < 0) clampedIndex = 0;
            if (clampedIndex >= _activeTeamAvatars.Count) clampedIndex = _activeTeamAvatars.Count - 1;
            prevSelectedAvatarIndex = clampedIndex;
        }
        _previousIndex = prevSelectedAvatarIndex;

        // Update properties.
        // Notify player.
        UpdateTeamProperties();

        // Send response packet.
        if (responsePacket != null)
        {
            _ = Player.SendPacket(responsePacket);
        }

        // Ensure new selected character index is alive.
        // If not, change to another alive one or revive.
        CheckCurrentAvatarIsAlive(currentEntity);
    }
    
    /// <summary>
    /// Ensures the currently selected avatar is alive. If not, switches to another alive avatar or revives.
    /// </summary>
    /// <param name="currentEntity">The current entity to check (can be null).</param>
    public void CheckCurrentAvatarIsAlive(EntityAvatar? currentEntity = null)
    {
        if (currentEntity == null)
        {
            currentEntity = GetCurrentAvatarEntity();
        }
        
        if (_activeTeamAvatars.Count == 0 || _previousIndex < 0 || _previousIndex >= _activeTeamAvatars.Count)
            return;

        // Ensure currently selected character is still alive
        if (_activeTeamAvatars[_previousIndex].LifeState != 1)
        {
            // Character died in a dungeon challenge...
            // TODO: Implement getDeadAvatarReplacement logic
            // For now, just switch to first alive avatar or first if none are alive
            int replaceIndex = GetDeadAvatarReplacement();
            if (0 <= replaceIndex && replaceIndex < _activeTeamAvatars.Count)
            {
                _previousIndex = replaceIndex;
            }
            else
            {
                // Team wiped in dungeon...
                // Revive and change to first avatar.
                _previousIndex = 0;
                // TODO: Implement reviveAvatar
                // ReviveAvatar(GetCurrentAvatarEntity()?.Avatar);
            }
        }

        // Check if character changed
        var newAvatarEntity = GetCurrentAvatarEntity();
        if (currentEntity != null && newAvatarEntity != null && currentEntity != newAvatarEntity)
        {
            // TODO: Call PlayerSwitchAvatarEvent.
            // var event = new PlayerSwitchAvatarEvent(Player, currentEntity.Avatar, newAvatarEntity.Avatar);
            // if (!event.Call()) return;

            // Remove and Add
            // TODO: Implement ReplaceEntity
            // Player.Scene?.ReplaceEntity(currentEntity, newAvatarEntity);
        }
    }
    
    /// <summary>
    /// Gets a replacement index for a dead avatar. Returns first alive avatar index, or -1 if none are alive.
    /// </summary>
    private int GetDeadAvatarReplacement()
    {
        for (int i = 0; i < _activeTeamAvatars.Count; i++)
        {
            if (_activeTeamAvatars[i].LifeState == 1)
            {
                return i;
            }
        }
        return -1;
    }
    
    /// <summary>
    /// Checks if the player is currently using a trial team.
    /// </summary>
    public bool IsUsingTrialTeam()
    {
        return _usingTrialTeam;
    }
    
    /// <summary>
    /// Gets the trial avatars dictionary (avatarId -> avatarGuid mapping).
    /// </summary>
    public Dictionary<int, uint> GetTrialAvatars()
    {
        if (_trialAvatars == null)
            _trialAvatars = [];
        return _trialAvatars;
    }
    
    /// <summary>
    /// Gets a trial avatar by its ID.
    /// </summary>
    /// <param name="avatarId">The avatar ID to look up.</param>
    /// <returns>The avatar GUID if found, otherwise 0.</returns>
    public uint GetTrialAvatarGuid(int avatarId)
    {
        if (_trialAvatars != null && _trialAvatars.TryGetValue(avatarId, out var guid))
            return guid;
        return 0;
    }
    
    /// <summary>
    /// Gets the trial avatar data info by avatar ID.
    /// </summary>
    /// <param name="avatarId">The avatar ID to look up.</param>
    /// <returns>The AvatarDataInfo if found, otherwise null.</returns>
    public AvatarDataInfo? GetTrialAvatar(int avatarId)
    {
        var guid = GetTrialAvatarGuid(avatarId);
        if (guid == 0)
            return null;
        return Player.AvatarManager.GetAvatarByGuid(guid);
    }
    
    // Additional methods for multiplayer, trial avatars, temporary teams, etc. will be added later
}
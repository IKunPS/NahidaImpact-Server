using System;
using System.Threading.Tasks;
using NahidaImpact.Database.Avatar;
using NahidaImpact.Database.Repositories;
using NahidaImpact.Database.Team;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.GameServer.Server.Packet.Send.Avatar;
using NahidaImpact.GameServer.Server.Packet.Send.Scene;
using NahidaImpact.KcpSharp;
using NahidaImpact.Util;

namespace NahidaImpact.GameServer.Game.Player.Team
{
    public class TeamManager : BasePlayerManager
    {
        private static readonly Logger Logger = new("TeamManager");
        private readonly ITeamRepository _teamRepository;
        private readonly object _syncRoot = new object(); // 线程同步对象

        private TeamData _teamData;
        private readonly Task _initializationTask;
        private List<EntityAvatar> _activeTeamAvatars;
        private TeamInfo _mpTeam;
        private int _useTemporarilyTeamIndex = -1;
        private List<TeamInfo> _temporaryTeams;
        private bool _usingTrialTeam;
        private TeamInfo? _trialAvatarTeam;
        private Dictionary<int, uint> _trialAvatars; // avatarId -> avatarGuid
        private int _previousIndex = -1;

        private EntityTeam? _entity;

        // 外部可通过此属性访问当前队伍数据（只读）
        public TeamData TeamData => _teamData;

        public TeamManager(PlayerInstance player, ITeamRepository teamRepository) : base(player)
        {
            _teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
            _activeTeamAvatars = new List<EntityAvatar>();
            _trialAvatars = new Dictionary<int, uint>();
            _temporaryTeams = new List<TeamInfo>();
            _mpTeam = new TeamInfo();
            _trialAvatarTeam = new TeamInfo();

            // 异步初始化，但构造函数不能 async，所以使用 Fire-and-forget 并捕获异常
            _initializationTask = InitializeAsync();
            _initializationTask.ContinueWith(t =>
            {
                if (t.IsFaulted)
                    Logger.Error($"Failed to initialize TeamManager for player {player.Uid}", t.Exception);
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        private async Task InitializeAsync()
        {
            // 加载或创建数据库记录
            _teamData = await _teamRepository.GetOrCreateAsync(Player.Uid);

            // 加载内存数据
            _mpTeam = _teamData.MpTeam ?? new TeamInfo();
            _temporaryTeams = _teamData.TemporaryTeams ?? new List<TeamInfo>();
            _usingTrialTeam = _teamData.UsingTrialTeam;
            _trialAvatarTeam = _teamData.TrialAvatarTeam;
            _trialAvatars = _teamData.TrialAvatars ?? new Dictionary<int, uint>();
            _previousIndex = _teamData.PreviousIndex;
            _useTemporarilyTeamIndex = _teamData.UseTemporarilyTeamIndex;

            // 确保数据一致性
            EnsureTeamsConsistency();

            // 保存一次以确保所有字段不为 null
            await SaveChangesAsync();
        }

        private void EnsureInitialized()
        {
            if (_teamData == null)
            {
                _initializationTask.GetAwaiter().GetResult();
            }
        }

        private void EnsureTeamsConsistency()
        {
            if (_teamData.Teams.Count == 0)
            {
                for (uint i = 1; i <= GameConstants.DEFAULT_TEAMS; i++)
                    _teamData.Teams.Add(new TeamInfo { Index = i });
                _teamData.CurrentTeamIndex = 1;
            }

            if (_mpTeam == null)
                _mpTeam = new TeamInfo();
            if (_temporaryTeams == null)
                _temporaryTeams = new List<TeamInfo>();
            if (_trialAvatars == null)
                _trialAvatars = new Dictionary<int, uint>();
            if (_trialAvatarTeam == null)
                _trialAvatarTeam = new TeamInfo();

            // 确保当前队伍索引有效
            if (_teamData.CurrentTeamIndex < 1 || _teamData.CurrentTeamIndex > _teamData.Teams.Count)
                _teamData.CurrentTeamIndex = 1;

            // 确保临时队伍索引有效
            if (_useTemporarilyTeamIndex < -1 || _useTemporarilyTeamIndex >= _temporaryTeams.Count)
                _useTemporarilyTeamIndex = -1;
        }

        // 统一保存方法，由业务层在合适时机调用（例如玩家下线时）
        public async Task SaveChangesAsync()
        {
            lock (_syncRoot)
            {
                // 将内存状态写回 _teamData
                _teamData.MpTeam = _mpTeam;
                _teamData.TemporaryTeams = _temporaryTeams;
                _teamData.UsingTrialTeam = _usingTrialTeam;
                _teamData.TrialAvatarTeam = _trialAvatarTeam;
                _teamData.TrialAvatars = _trialAvatars;
                _teamData.PreviousIndex = _previousIndex;
                _teamData.UseTemporarilyTeamIndex = _useTemporarilyTeamIndex;
            }
            await _teamRepository.UpdateAsync(_teamData);
        }

        // 以下方法均为业务逻辑，直接操作内存状态，最后调用 SaveChangesAsync
        // 若不想每次修改都保存，可将 SaveChangesAsync 调用移到外部（如玩家下线时）

        public List<GameAvatarTeam> GetAvatarTeams()
        {
            lock (_syncRoot)
            {
                EnsureInitialized();
                return _teamData.Teams.Select(t => new GameAvatarTeam
                {
                    Index = t.Index,
                    Name = t.Name,
                    AvatarGuidList = t.AvatarGuidList.ToList() // 深拷贝，防止外部修改
                }).ToList();
            }
        }

        public uint CurrentTeamIndex
        {
            get
            {
                EnsureInitialized();
                return _teamData.CurrentTeamIndex;
            }
        }

        // 获取当前队伍信息的核心方法（返回内部对象，调用者需自行处理并发）
        private TeamInfo GetCurrentTeamInfoCore()
        {
            EnsureInitialized();
            if (_useTemporarilyTeamIndex >= 0 && _useTemporarilyTeamIndex < _temporaryTeams.Count)
                return _temporaryTeams[_useTemporarilyTeamIndex];
            if (Player.IsInMultiplayer())
                return _mpTeam;
            var team = _teamData.Teams.FirstOrDefault(t => t.Index == _teamData.CurrentTeamIndex);
            return team ?? _teamData.Teams[0];
        }

        public GameAvatarTeam GetCurrentTeamInfo()
        {
            lock (_syncRoot)
            {
                var team = GetCurrentTeamInfoCore();
                return new GameAvatarTeam
                {
                    Index = team.Index,
                    Name = team.Name,
                    AvatarGuidList = team.AvatarGuidList.ToList()
                };
            }
        }

        internal TeamInfo GetCurrentTeamInfoInternal()
        {
            lock (_syncRoot)
            {
                return GetCurrentTeamInfoCore();
            }
        }

        public List<EntityAvatar> GetActiveTeam()
        {
            lock (_syncRoot)
            {
                return _activeTeamAvatars.ToList(); // 返回副本
            }
        }

        public EntityAvatar? GetCurrentAvatarEntity()
        {
            lock (_syncRoot)
            {
                if (_activeTeamAvatars.Count == 0)
                    return null;
                int index = _previousIndex;
                if (index < 0 || index >= _activeTeamAvatars.Count)
                    index = 0;
                return _activeTeamAvatars[index];
            }
        }

        public int GetMaxTeamSize() => GameConstants.MAX_AVATARS_IN_TEAM;

        public bool CanAddAvatarToTeam(GameAvatarTeam team) => team.Size < GetMaxTeamSize();

        public bool AddAvatarToTeam(GameAvatarTeam team, ulong avatarGuid)
        {
            lock (_syncRoot)
            {
                if (!CanAddAvatarToTeam(team))
                    return false;
                if (team.Contains(avatarGuid))
                    return false;
                team.AddAvatar(avatarGuid);
                return true;
            }
        }

        public bool RemoveAvatarFromTeam(GameAvatarTeam team, int slot)
        {
            lock (_syncRoot)
            {
                return team.RemoveAvatar(slot);
            }
        }

        public void SetCurrentTeam(uint teamIndex)
        {
            lock (_syncRoot)
            {
                EnsureInitialized();
                if (teamIndex < 1 || teamIndex > _teamData.Teams.Count)
                    return;
                _teamData.CurrentTeamIndex = teamIndex;
            }
        }

        public async Task SetTeamNameAsync(uint teamIndex, string name)
        {
            lock (_syncRoot)
            {
                EnsureInitialized();
                var team = _teamData.Teams.FirstOrDefault(t => t.Index == teamIndex);
                if (team == null)
                    return;
                team.Name = name;
            }
            await SaveChangesAsync();
            await Player.SendPacket(new PacketChangeTeamNameRsp((int)teamIndex, name));
        }

        public bool AddAvatarToCurrentTeam(ulong avatarGuid)
        {
            lock (_syncRoot)
            {
                var currentTeam = GetCurrentTeamInfoCore();
                if (currentTeam.Contains(avatarGuid))
                    return false;
                if (currentTeam.Size >= GetMaxTeamSize())
                    return false;
                currentTeam.AddAvatar(avatarGuid);
                return true;
            }
        }

        // 更新队伍实体列表（场景中的实体）
        public async Task UpdateTeamEntitiesAsync(BasePacket? responsePacket = null)
        {
            TeamInfo currentTeamInfo;
            lock (_syncRoot)
            {
                currentTeamInfo = GetCurrentTeamInfoCore();
            }

            if (currentTeamInfo.AvatarGuidList.Count == 0)
                return;

            var existingAvatars = new Dictionary<int, EntityAvatar>();
            int prevSelectedAvatarIndex = -1;

            // 收集现有实体
            lock (_syncRoot)
            {
                foreach (var entity in _activeTeamAvatars)
                {
                    existingAvatars[(int)entity.AvatarInfo.AvatarId] = entity;
                }
                _activeTeamAvatars.Clear();
            }

            var newActiveTeam = new List<EntityAvatar>();
            var avatarsToRemove = new List<EntityAvatar>();

            for (int i = 0; i < currentTeamInfo.AvatarGuidList.Count; i++)
            {
                var avatarGuid = currentTeamInfo.AvatarGuidList[i];
                var avatar = Player.Avatars.FirstOrDefault(a => a.Guid == avatarGuid);
                if (avatar == null)
                {
                    Logger.Warn($"Avatar with guid {avatarGuid} not found for player {Player.Uid}");
                    continue;
                }

                var avatarId = avatar.AvatarId;
                EntityAvatar entity;
                if (existingAvatars.TryGetValue((int)avatarId, out var existing))
                {
                    entity = existing;
                    existingAvatars.Remove((int)avatarId);
                    if (entity == GetCurrentAvatarEntity())
                        prevSelectedAvatarIndex = i;
                }
                else
                {
                    // 创建新实体
                    entity = EntityCreationEvent.Call<EntityAvatar>(
                        new Type[] { typeof(PlayerInstance), typeof(AvatarDataInfo) },
                        new object[] { Player, avatar });
                    if (entity == null)
                    {
                        Logger.Warn($"Failed to create entity for avatar {avatarId} for player {Player.Uid}");
                        continue;
                    }
                }
                newActiveTeam.Add(entity);
            }

            // 移除不再使用的实体
            foreach (var entity in existingAvatars.Values)
            {
                Player.Scene?.RemoveEntity(entity);
                avatarsToRemove.Add(entity);
                // TODO: 保存 avatar 数据
            }

            // 更新活跃队伍列表
            lock (_syncRoot)
            {
                _activeTeamAvatars = newActiveTeam;
                if (_activeTeamAvatars.Count == 0)
                {
                    _previousIndex = -1;
                }
                else
                {
                    if (prevSelectedAvatarIndex == -1)
                    {
                        int clampedIndex = _previousIndex;
                        if (clampedIndex < 0) clampedIndex = 0;
                        if (clampedIndex >= _activeTeamAvatars.Count) clampedIndex = _activeTeamAvatars.Count - 1;
                        prevSelectedAvatarIndex = clampedIndex;
                    }
                    _previousIndex = prevSelectedAvatarIndex;
                }
            }

            // 更新队伍共鸣（空实现）
            UpdateTeamResonances();

            // 发送更新包
            await Player.SendPacket(new PacketSceneTeamUpdateNotify(Player));
            if (responsePacket != null)
                await Player.SendPacket(responsePacket);

            // 检查当前角色是否存活
            CheckCurrentAvatarIsAlive();
        }

        private void UpdateTeamResonances()
        {
            // TODO: 实现队伍共鸣逻辑
        }

        private void CheckCurrentAvatarIsAlive(EntityAvatar? currentEntity = null)
        {
            EntityAvatar? current;
            lock (_syncRoot)
            {
                if (_activeTeamAvatars.Count == 0 || _previousIndex < 0 || _previousIndex >= _activeTeamAvatars.Count)
                    return;
                current = _activeTeamAvatars[_previousIndex];
            }

            if (current.LifeState != 1)
            {
                int replaceIndex;
                lock (_syncRoot)
                {
                    replaceIndex = GetDeadAvatarReplacement();
                }
                if (replaceIndex >= 0 && replaceIndex < _activeTeamAvatars.Count)
                {
                    lock (_syncRoot)
                    {
                        _previousIndex = replaceIndex;
                    }
                }
                else
                {
                    // 全队阵亡，复活第一个角色
                    lock (_syncRoot)
                    {
                        _previousIndex = 0;
                        // TODO: 实现复活逻辑
                    }
                }
            }

            var newAvatarEntity = GetCurrentAvatarEntity();
            if (currentEntity != null && newAvatarEntity != null && currentEntity != newAvatarEntity)
            {
                // TODO: 切换角色事件
                // Player.Scene?.ReplaceEntity(currentEntity, newAvatarEntity);
            }
        }

        private int GetDeadAvatarReplacement()
        {
            for (int i = 0; i < _activeTeamAvatars.Count; i++)
            {
                if (_activeTeamAvatars[i].LifeState == 1)
                    return i;
            }
            return -1;
        }

        public TeamInfo GetMpTeam() => _mpTeam;

        public TeamInfo GetCurrentSinglePlayerTeamInfo()
        {
            lock (_syncRoot)
            {
                return GetCurrentTeamInfoCore();
            }
        }

        public int GetCurrentCharacterIndex()
        {
            lock (_syncRoot)
            {
                return _previousIndex;
            }
        }

        public void SetCurrentCharacterIndex(int index)
        {
            lock (_syncRoot)
            {
                if (index >= 0 && index < _activeTeamAvatars.Count)
                {
                    _previousIndex = index;
                    Player.EntityAvatar = GetCurrentAvatarEntity();
                }
            }
        }

        public bool IsUsingTrialTeam() => _usingTrialTeam;

        public Dictionary<int, uint> GetTrialAvatars()
        {
            lock (_syncRoot)
            {
                return new Dictionary<int, uint>(_trialAvatars);
            }
        }

        public uint GetTrialAvatarGuid(int avatarId)
        {
            lock (_syncRoot)
            {
                return _trialAvatars.TryGetValue(avatarId, out var guid) ? guid : 0;
            }
        }

        public AvatarDataInfo? GetTrialAvatar(int avatarId)
        {
            var guid = GetTrialAvatarGuid(avatarId);
            if (guid == 0)
                return null;
            return Player.AvatarManager.GetAvatarByGuid(guid);
        }

        // 公开实体属性
        public EntityTeam? Entity
        {
            get => _entity;
            set => _entity = value;
        }

        public void SetEntity(EntityTeam entity)
        {
            _entity = entity;
        }
    }
}
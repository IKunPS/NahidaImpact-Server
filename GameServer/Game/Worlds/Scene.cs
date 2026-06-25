using System;
using NahidaImpact.Data;
using NahidaImpact.Data.Excel;
using NahidaImpact.Database.Avatar;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.GameServer.Game.Entity.Gadget;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.Enums.Scene;
using NahidaImpact.Internationalization;
using NahidaImpact.KcpSharp;
using System.Collections.Concurrent;
using NahidaImpact.GameServer.Server.Packet.Send.Scene;
using NahidaImpact.GameServer.Server.Packet.Send.Time;
using NahidaImpact.Prop;
using NahidaImpact.Util;

namespace NahidaImpact.GameServer.Game.Worlds;

public class Scene
{
    public static Logger Logger { get; } = new("Scene");
    public World World { get; }
    public SceneDataExcel SceneData { get; }
    public int Id => (int)SceneData.Id;
    public SceneTypeEnum SceneType => SceneData.SceneType;
    
    private readonly List<PlayerInstance> _players = [];
    private readonly object _playerListLock = new object();
    private readonly ConcurrentDictionary<int, BaseEntity> _entities = new();
    public ConcurrentDictionary<int, BaseEntity> WeaponEntities { get; } = new();
    private readonly HashSet<int> _spawnedEntities = [];
    private readonly HashSet<int> _deadSpawnedEntities = [];
    private readonly HashSet<int> _loadedBlocks = [];
    private readonly HashSet<int> _loadedGroups = [];
    
    private bool _finishedLoading = false;
    private int _tickCount = 0;
    private bool _isPaused = false;
    private readonly long _sceneStartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

    /// <summary>Elapsed time in milliseconds since this scene started.</summary>
    public long SceneTime => DateTimeOffset.Now.ToUnixTimeMilliseconds() - _sceneStartTime;

    /// <summary>Whether the scene is currently paused.</summary>
    public bool IsPaused => _isPaused;

    public Scene(World world, SceneDataExcel sceneData)
    {
        World = world;
        SceneData = sceneData;
    }

    // Prevents scene from being cleaned up when empty, used during same-scene teleport.
    public bool DontDestroyWhenEmpty { get; set; }

    // Tracks the previous scene point for cross-scene transition logic.
    public int PrevScenePoint { get; set; }

    #region Player Management

    public int PlayerCount
    {
        get
        {
            lock (_playerListLock)
            {
                return _players.Count;
            }
        }
    }

    public PlayerInstance? Host => World.Host;

    public IReadOnlyList<PlayerInstance> Players
    {
        get
        {
            lock (_playerListLock)
            {
                return _players.ToList().AsReadOnly();
            }
        }
    }
    
    public void AddPlayer(PlayerInstance player)
    {
        // Parameter validation
        if (player == null)
            throw new ArgumentNullException(nameof(player), "Player cannot be null");
        
        // Check if player already in (quick check without lock)
        if (_players.Contains(player))
            return;
        
        // CRITICAL: Remove from previous scene OUTSIDE of any lock to prevent circular dependency
        if (player.Scene != null && player.Scene != this)
            player.Scene.RemovePlayer(player);
        
        // Now add to this scene
        AddPlayerInternal(player);
    }
    
    private void AddPlayerInternal(PlayerInstance player)
    {
        // Register player with fine-grained locking
        lock (_playerListLock)
        {
            if (_players.Contains(player))
                return; // Already in this scene
            
            _players.Add(player);
            player.SceneId = (uint)Id;
            player.Scene = this;
        }
        
        SetupPlayerAvatars(player);
        SpawnSceneGadgets(player);
    }

    public void RemovePlayer(PlayerInstance player)
    {
        // Parameter validation
        if (player == null)
            throw new ArgumentNullException(nameof(player), "Player cannot be null");
        
        // Check if player is in this scene (quick check without lock)
        if (!_players.Contains(player))
            return;
        
        // Critical section with fine-grained lock
        lock (_playerListLock)
        {
            if (!_players.Contains(player))
                return;
            
            _players.Remove(player);
            player.Scene = null;
        }
        
        // Remove player avatars outside lock to minimize lock time
        RemovePlayerAvatars(player);
        
        if (PlayerCount == 0 && !DontDestroyWhenEmpty)
            World.DeregisterScene(this);
    }
    
    public void SpawnPlayer(PlayerInstance player)
    {
        var teamManager = player.TeamManager;
        if (teamManager == null) return;

        var currentAvatarEntity = teamManager.GetCurrentAvatarEntity();
        if (currentAvatarEntity == null) return;

        // hk4e: revive dead avatars and heal to full on scene enter
        foreach (var entity in teamManager.GetActiveTeam())
        {
            if (!entity.IsAlive())
            {
                entity.Heal(entity.GetFightProperty(FightProp.FIGHT_PROP_MAX_HP));
                entity.IsDead = false;
                entity.LifeState = 1;
            }
        }

        AddEntity(currentAvatarEntity);
    }
    
    private void SetupPlayerAvatars(PlayerInstance player)
    {
        var teamManager = player.TeamManager;
        if (teamManager == null)
        {
            Logger.Warn(I18NManager.Translate("Game.SceneInfo.NoTeamManagerSetup", player.Uid.ToString()));
            return;
        }
        
        // Clear entities from old team
        teamManager.GetActiveTeam().Clear();
        
        // Add new entities for player
        var teamInfo = teamManager.CurrentSinglePlayerTeamInfo;
        foreach (var avatarGuid in teamInfo.AvatarGuidList)
        {
            var avatar = player.AvatarManager.GetAvatarByGuid(avatarGuid);
            if (avatar == null)
            {
                if (teamManager.UsingTrialTeam)
                {
                    // In trial team, we need to find avatar by ID from trial avatars
                    // Since we have GUIDs in the team, we can try to get by GUID first
                    // If not found, we need to get avatar ID from trial avatars dictionary
                    // For now, we'll skip as trial team logic is more complex
                    continue;
                }
                continue;
            }
            
            var entity = EntityFactory.Create<EntityAvatar>(
                new Type[] { typeof(Scene), typeof(AvatarDataInfo) },
                new object[] { this, avatar });
            
            if (entity != null)
            {
                teamManager.GetActiveTeam().Add(entity);
            }
        }
        
        // Clamp character index when team size changed
        var currentIndex = teamManager.CurrentCharacterIndex;
        var teamCount = teamManager.GetActiveTeam().Count;
        if (teamCount == 0)
        {
            teamManager.SetCurrentCharacterIndex(0);
        }
        else if (currentIndex >= teamCount || currentIndex < 0)
        {
            teamManager.SetCurrentCharacterIndex(teamCount - 1);
        }
    }
    
    private void RemovePlayerAvatars(PlayerInstance player)
    {
        var toRemove = _entities.Values
            .Where(e => e.Owner == player && e is EntityAvatar)
            .ToList();
        foreach (var entity in toRemove)
            RemoveEntity(entity, VisionType.Die);

        // Cleanup weapon entities
        var weaponIds = WeaponEntities.Values
            .Where(w => w.Owner == player)
            .Select(w => (int)w.Id)
            .ToList();
        foreach (var id in weaponIds)
            WeaponEntities.TryRemove(id, out _);
    }
    
    #endregion
    
    #region Entity Management
    
    public BaseEntity? GetEntityById(int id)
    {
        if (_entities.TryGetValue(id, out var entity))
            return entity;
        if (WeaponEntities.TryGetValue(id, out entity))
            return entity;
        return null;
    }

    public List<BaseEntity> GetEntitiesByConfigId(int configId)
    {
        var result = new List<BaseEntity>();
        foreach (var kv in _entities)
            if (kv.Value.ConfigId == configId)
                result.Add(kv.Value);
        return result;
    }

    public void AddEntity(BaseEntity entity)
    {
        if (entity == null)
        {
            Logger.Warn(I18NManager.Translate("Game.SceneInfo.NullEntity", Id.ToString()));
            return;
        }
        
        AddEntityDirectly(entity);
        BroadcastPacket(new PacketSceneEntityAppearNotify(entity));
    }

    public void AddEntityDirectly(BaseEntity entity) {
        _entities[(int)entity.Id] = entity;
        entity.OnCreate();
    }

    // Add multiple entities and broadcast a single appear notify per chunk (mirrors Java addEntities).
    public void AddEntities(IEnumerable<BaseEntity> entities, VisionType visionType = VisionType.Born)
    {
        var list = entities.ToList();
        if (list.Count == 0) return;

        foreach (var entity in list)
            AddEntityDirectly(entity);

        // Chunk to avoid oversized packets; matches Java's 100-entity batch limit.
        foreach (var chunk in Chunk(list, 100))
            BroadcastPacket(new PacketSceneEntityAppearNotify(chunk, visionType));
    }

    private static IEnumerable<List<T>> Chunk<T>(List<T> source, int size)
    {
        for (int i = 0; i < source.Count; i += size)
            yield return source.GetRange(i, Math.Min(size, source.Count - i));
    }
    
    public void RemoveEntity(BaseEntity entity, VisionType visionType = VisionType.Die)
    {
        BaseEntity removed = null;
        if (_entities.TryRemove((int)entity.Id, out removed))
        {
            removed.OnRemoved();
        }
        if (removed != null)
        {
            BroadcastPacket(new PacketSceneEntityDisappearNotify(removed, visionType));
        }
    }
    
    public void ReplaceEntity(BaseEntity oldEntity, BaseEntity newEntity)
    {
        if (_entities.TryRemove((int)oldEntity.Id, out var removed))
            removed.OnRemoved();
        _entities[(int)newEntity.Id] = newEntity;
        newEntity.OnCreate();
        BroadcastPacket(new PacketSceneEntityDisappearNotify(oldEntity, VisionType.Replace));
        BroadcastPacket(new PacketSceneEntityAppearNotify(newEntity, VisionType.Replace, (int)oldEntity.Id));
    }

    public void RemoveEntities(IEnumerable<BaseEntity> entities, VisionType visionType = VisionType.Die)
    {
        foreach (var entity in entities) RemoveEntity(entity, visionType);
    }
    
    public void UpdateEntity(BaseEntity entity)
    {
        BroadcastPacket(new PacketSceneEntityAppearNotify([entity], VisionType.Refresh));
    }

    // Show all scene entities (except the player's current avatar) to a player on scene enter.
    // Mirrors Java showOtherEntities; Rebornable CD filter omitted (no such entities yet).
    public void ShowOtherEntities(PlayerInstance player)
    {
        var currentEntity = player.TeamManager?.GetCurrentAvatarEntity();
        var entities = _entities.Values.ToArray()
            .Where(e => e != currentEntity)
            .ToList();

        if (entities.Count == 0) return;
        _ = player.SendPacket(new PacketSceneEntityAppearNotify(entities, VisionType.Meet));
    }

    public void KillEntity(BaseEntity entity, int killerId = 0)
    {
        if (entity.IsDead) return;

        entity.Damage(float.MaxValue, killerId);
        if (entity is EntityMonster || entity is EntityBaseGadget)
        {
            RemoveEntity(entity, VisionType.Die);
            // Drop rewards deferred to DropManager implementation
        }
    }

    // Spawn a single gadget at position
    public EntityGadget SpawnGadget(int gadgetId, Position pos, Position? rot = null,
        GadgetContent? content = null)
    {
        var gadget = new EntityGadget(this, gadgetId, pos, rot, content);
        AddEntity(gadget);
        return gadget;
    }

    public void SpawnSceneGadgets(PlayerInstance player)
    {
        if (SceneData == null) return;

        var sceneId = (int)SceneData.Id;
        // Full implementation requires a .scn binary parser for block/group data,
        // which defines monster spawns, gadget placements, and zone triggers.
        // For now, scene points (waypoints, statues, domains) are handled by
        // client-side map UI; this is a placeholder for server-side gadget spawning.
        foreach (var pointId in GameData.ScenePointsPerScene.GetValueOrDefault(sceneId, []))
        {
            var entry = GameData.GetScenePointEntryById(sceneId, pointId);
            if (entry?.PointData?.Pos != null)
            {
                _loadedGroups.Add(pointId);
            }
        }
    }

    // hk4e Scene block/group tracking
    public bool IsBlockLoaded(int blockId) => _loadedBlocks.Contains(blockId);
    public void MarkBlockLoaded(int blockId) => _loadedBlocks.Add(blockId);
    public bool IsGroupLoaded(int groupId) => _loadedGroups.Contains(groupId);
    public void MarkGroupLoaded(int groupId) => _loadedGroups.Add(groupId);

    public void UnloadBlock(int blockId)
    {
        _loadedBlocks.Remove(blockId);
        var toRemove = _entities.Values.Where(e => e.BlockId == blockId).ToList();
        foreach (var entity in toRemove)
            RemoveEntity(entity, VisionType.Remove);
    }

    #endregion
    
    #region Broadcasting
    
    public void BroadcastPacket(BasePacket packet)
    {
        List<PlayerInstance> snapshot;
        lock (_playerListLock)
        {
            snapshot = _players.ToList();
        }
        foreach (var player in snapshot)
            _ = player.SendPacket(packet);
    }

    public void BroadcastPacketToOthers(PlayerInstance excludedPlayer, BasePacket packet)
    {
        List<PlayerInstance> snapshot;
        lock (_playerListLock)
        {
            snapshot = _players.ToList();
        }
        foreach (var player in snapshot)
        {
            if (player == excludedPlayer)
                continue;
            _ = player.SendPacket(packet);
        }
    }
    
    #endregion
    
    #region Scene Lifecycle
    
    public void OnTick()
    {
        if (_isPaused) return;
        _tickCount++;

        // Flush combat invoke entries for all players
        List<PlayerInstance> snapshot;
        lock (_playerListLock) snapshot = _players.ToList();
        foreach (var player in snapshot)
        {
            player.CombatInvokeHandler?.Send();
            player.AbilityManager?.AbilityInvokeHandler.Send();
            player.AbilityManager?.ClientAbilityInitFinishHandler.Send();
        }

        // Tick all entities (monsters, gadgets, etc.)
        foreach (var entity in _entities.Values)
            entity.OnTick(_tickCount);
    }
    
    public void SetPaused(bool paused)
    {
        if (_isPaused != paused)
        {
            _isPaused = paused;
            List<PlayerInstance> snapshot;
            lock (_playerListLock) snapshot = _players.ToList();
            foreach (var player in snapshot)
                _ = player.SendPacket(new PacketSceneTimeNotify(player));
        }
    }
    
    public void FinishLoading()
    {
        if (_finishedLoading)
            return;
        _finishedLoading = true;
    }
    
    #endregion
    
    #region Helper Methods
    
    public bool IsInScene(BaseEntity entity) => _entities.ContainsKey((int)entity.Id);

    public IEnumerable<BaseEntity> GetEntities() => _entities.Values;
    
    #endregion
}
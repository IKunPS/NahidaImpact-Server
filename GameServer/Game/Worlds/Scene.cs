using System;
using NahidaImpact.Data;
using NahidaImpact.Data.Excel;
using NahidaImpact.Database.Avatar;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.Enums.Scene;
using NahidaImpact.Internationalization;
using NahidaImpact.KcpSharp;
using System.Collections.Concurrent;
using NahidaImpact.GameServer.Server.Packet.Send.Scene;
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
    private bool _dontDestroyWhenEmpty = false;
    private int _prevScenePoint = 0;

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
    public void SetDontDestroyWhenEmpty(bool value) => _dontDestroyWhenEmpty = value;
    public bool GetDontDestroyWhenEmpty() => _dontDestroyWhenEmpty;

    // Tracks the previous scene point for cross-scene transition logic.
    public void SetPrevScenePoint(int point) => _prevScenePoint = point;
    public int GetPrevScenePoint() => _prevScenePoint;
    
    #region Player Management
    
    public int GetPlayerCount()
    {
        lock (_playerListLock)
        {
            return _players.Count;
        }
    }
    
    public PlayerInstance? GetHost() => World.GetHost();
    
    public IReadOnlyList<PlayerInstance> GetPlayers()
    {
        lock (_playerListLock)
        {
            return _players.ToList().AsReadOnly();
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
        
        // Setup avatars and gadgets outside lock to minimize lock time
        SetupPlayerAvatars(player);
        
        // TODO: Create all gadgets from scene build
        // this.sceneBuild.createAllGadget(player);
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
        
        if (GetPlayerCount() == 0 && !_dontDestroyWhenEmpty)
            World.DeregisterScene(this);
    }
    
    public void SpawnPlayer(PlayerInstance player)
    {
        var teamManager = player.TeamManager;
        if (teamManager == null)
        {
            Logger.Warn(I18NManager.Translate("Game.SceneInfo.NoTeamManagerSpawn", player.Uid.ToString()));
            return;
        }
        
        var currentAvatarEntity = teamManager.GetCurrentAvatarEntity();
        if (currentAvatarEntity == null)
        {
            Logger.Warn(I18NManager.Translate("Game.SceneInfo.NoAvatarEntity", player.Uid.ToString()));
            return;
        }
            
        // TODO: Check HP and heal if necessary (like Java version does)
        // For now, just add the entity to scene

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
        var teamInfo = teamManager.GetCurrentSinglePlayerTeamInfo();
        foreach (var avatarGuid in teamInfo.AvatarGuidList)
        {
            var avatar = player.AvatarManager.GetAvatarByGuid(avatarGuid);
            if (avatar == null)
            {
                if (teamManager.IsUsingTrialTeam())
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
        var currentIndex = teamManager.GetCurrentCharacterIndex();
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
        // TODO: Implement avatar removal
        // Remove all entities belonging to this player
        var toRemove = _entities.Values.Where(e => e.Owner == player).ToList();
        foreach (var entity in toRemove)
            RemoveEntity(entity, VisionType.Die);
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
        // TODO: Broadcast entity update packet
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
        if (_isPaused)
            return;

        _tickCount++;

        // Snapshot to avoid collection-modified-during-enumeration
        List<PlayerInstance> snapshot;
        lock (_playerListLock)
        {
            snapshot = _players.ToList();
        }
        foreach (var player in snapshot)
        {
            player.CombatInvokeHandler?.Send();
        }
    }
    
    public void SetPaused(bool paused)
    {
        if (_isPaused != paused)
        {
            _isPaused = paused;
            // TODO: Broadcast scene time notify
        }
    }
    
    public void FinishLoading()
    {
        if (_finishedLoading)
            return;
        _finishedLoading = true;
        // TODO: Invoke callbacks
    }
    
    #endregion
    
    #region Helper Methods
    
    public bool IsInScene(BaseEntity entity) => _entities.ContainsKey((int)entity.Id);
    
    #endregion
}
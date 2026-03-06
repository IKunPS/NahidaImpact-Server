using System;
using NahidaImpact.Data;
using NahidaImpact.Data.Excel;
using NahidaImpact.Database.Avatar;
using NahidaImpact.GameServer.Game.Avatar;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.Common.Enums.Scene;
using NahidaImpact.Proto;
using NahidaImpact.GameServer.Server.Packet;
using NahidaImpact.KcpSharp;
using System.Collections.Concurrent;
using NahidaImpact.GameServer.Server.Packet.Send.Scene;
using NahidaImpact.Util;

namespace NahidaImpact.GameServer.Game.World;

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
    private readonly ConcurrentDictionary<int, BaseEntity> _weaponEntities = new();
    private readonly HashSet<int> _spawnedEntities = [];
    private readonly HashSet<int> _deadSpawnedEntities = [];
    private readonly HashSet<int> _loadedBlocks = [];
    private readonly HashSet<int> _loadedGroups = [];
    
    private bool _finishedLoading = false;
    private int _tickCount = 0;
    private bool _isPaused = false;
    
    public Scene(World world, SceneDataExcel sceneData)
    {
        World = world;
        SceneData = sceneData;
    }
    
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
    
    /// <summary>
    /// Adds a player to this scene. Removes from previous scene if necessary.
    /// Fixed to prevent deadlock by removing player from old scene BEFORE acquiring lock.
    /// </summary>
    /// <param name="player">The player to add to this scene.</param>
    /// <exception cref="ArgumentNullException">Thrown when player is null.</exception>
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
    
    /// <summary>
    /// Internal method to add a player to this scene with fine-grained locking.
    /// </summary>
    /// <param name="player">The player to add.</param>
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
    
    /// <summary>
    /// Removes a player from this scene.
    /// Fixed to prevent deadlock by using fine-grained locking and not holding locks
    /// when calling methods on other objects.
    /// </summary>
    /// <param name="player">The player to remove from this scene.</param>
    /// <exception cref="ArgumentNullException">Thrown when player is null.</exception>
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
        
        if (GetPlayerCount() == 0)
            World.DeregisterScene(this);
    }
    
    public void SpawnPlayer(PlayerInstance player)
    {
        var teamManager = player.TeamManager;
        if (teamManager == null)
        {
            Logger.Warn($"Player {player.Uid} has no TeamManager, cannot spawn avatar");
            return;
        }
        
        var currentAvatarEntity = teamManager.GetCurrentAvatarEntity();
        if (currentAvatarEntity == null)
        {
            Logger.Warn($"Player {player.Uid} has no current avatar entity to spawn");
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
            Logger.Warn($"Player {player.Uid} has no TeamManager, cannot setup avatars");
            return;
        }
        
        // Clear entities from old team
        teamManager.GetActiveTeam().Clear();
        
        // Add new entities for player
        var teamInfo = teamManager.GetCurrentTeamInfoInternal();
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
            
            var entity = EntityCreationEvent.Call<EntityAvatar>(
                new Type[] { typeof(PlayerInstance), typeof(AvatarDataInfo) },
                new object[] { player, avatar });
            
            if (entity != null)
            {
                teamManager.GetActiveTeam().Add(entity);
            }
        }
        
        // Limit character index in case it's out of bounds
        var currentIndex = teamManager.GetCurrentCharacterIndex();
        if (currentIndex >= teamManager.GetActiveTeam().Count || currentIndex < 0)
        {
            teamManager.SetCurrentCharacterIndex(currentIndex - 1);
        }
    }
    
    private void RemovePlayerAvatars(PlayerInstance player)
    {
        // TODO: Implement avatar removal
        // Remove all entities belonging to this player
        var toRemove = _entities.Values.Where(e => e.Owner == player).ToList();
        foreach (var entity in toRemove)
            RemoveEntity(entity, VisionType.VisionDie);
    }
    
    #endregion
    
    #region Entity Management
    
    public BaseEntity? GetEntityById(int id)
    {
        if (_entities.TryGetValue(id, out var entity))
            return entity;
        if (_weaponEntities.TryGetValue(id, out entity))
            return entity;
        return null;
    }

    public void AddEntity(BaseEntity entity)
    {
        if (entity == null)
        {
            Logger.Warn($"Attempted to add null entity to scene {Id}");
            return;
        }
        
        AddEntityDirectly(entity);
        BroadcastPacket(new PacketSceneEntityAppearNotify(entity));
    }

    private void AddEntityDirectly(BaseEntity entity) {
        _entities[(int)entity.Id] = entity;
        entity.OnCreate(); // Call entity create event
    }
    
    public void RemoveEntity(BaseEntity entity, VisionType visionType = VisionType.VisionDie)
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
    
    public void RemoveEntities(IEnumerable<BaseEntity> entities, VisionType visionType = VisionType.VisionDie)
    {
        foreach (var entity in entities) RemoveEntity(entity, visionType);
    }
    
    public void UpdateEntity(BaseEntity entity)
    {
        // TODO: Broadcast entity update packet
    }
    
    #endregion
    
    #region Broadcasting
    
    public void BroadcastPacket(BasePacket packet)
    {
        foreach (var player in _players)
            _ = player.SendPacket(packet);
    }
    
    public void BroadcastPacketToOthers(PlayerInstance excludedPlayer, BasePacket packet)
    {
        foreach (var player in _players)
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
        
        // TODO: Implement tick logic (spawns, blocks, groups, etc.)
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
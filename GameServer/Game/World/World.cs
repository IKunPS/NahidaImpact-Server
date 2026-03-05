using System;
using NahidaImpact.Common.Enums;
using NahidaImpact.Util;
using NahidaImpact.Data;
using NahidaImpact.Data.Excel;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.GameServer.Game.Player;
using System.Collections.Concurrent;
using NahidaImpact.Enums;
using NahidaImpact.KcpSharp;
using NahidaImpact.GameServer.Server.Packet;
using NahidaImpact.GameServer.Server.Packet.Send.Player;

namespace NahidaImpact.GameServer.Game.World;

public class World
{
    public static Logger Logger { get; } = new("World");
    public PlayerInstance Host { get; private set; }
    public EntityWorld Entity { get; private set; }
    private readonly List<PlayerInstance> _players = [];
    private readonly ConcurrentDictionary<int, Scene> _scenes = new();
    private readonly object _playerListLock = new object();
    private int _nextEntityId = 0;
    private uint _nextPeerId = 0;
    private int _worldLevel;
    private bool _isMultiplayer = false;
    
    public World(PlayerInstance host, bool isMultiplayer = false)
    {
        Host = host;
        _worldLevel = host.Data.WorldLevel;
        _isMultiplayer = isMultiplayer;
        
        Entity = new EntityWorld(this);
        // Register world in server? TODO
    }
    
    public Scene? GetSceneById(int sceneId)
    {
        if (_scenes.TryGetValue(sceneId, out var scene))
            return scene;
        
        // Create scene from scene data if it doesn't exist
        if (GameData.SceneData.TryGetValue(sceneId, out var sceneData))
        {
            scene = new Scene(this, sceneData);
            RegisterScene(scene);
            return scene;
        }
        
        return null;
    }
    
    public void RegisterScene(Scene scene)
    {
        _scenes[scene.Id] = scene;
    }
    
    public void DeregisterScene(Scene scene)
    {
        _scenes.TryRemove(scene.Id, out _);
        // TODO: Cleanup scene resources
    }
    
    /// <summary>
    /// Internal method to add a player to this world with fine-grained locking.
    /// </summary>
    /// <param name="player">The player to add.</param>
    /// <param name="newSceneId">Optional scene ID (use -1 to use player's current scene).</param>
    private void AddPlayerInternal(PlayerInstance player, int newSceneId)
    {
        // Register player with fine-grained locking
        lock (_playerListLock)
        {
            if (_players.Contains(player))
                return; // Already in this world
            
            player.World = this;
            _players.Add(player);
        }
        
        // Set player variables (outside lock to minimize lock time)
        player.PeerId = GetNextPeerId();
        
        // Set team manager entity
        player.TeamManager?.SetEntity(new EntityTeam(player));
        
        // Copy main team to multiplayer team if in multiplayer
        if (_isMultiplayer)
        {
            var teamManager = player.TeamManager;
            if (teamManager != null)
            {
                var mpTeam = teamManager.GetMpTeam();
                var singlePlayerTeam = teamManager.GetCurrentSinglePlayerTeamInfo();
                var maxTeamSize = teamManager.GetMaxTeamSize();
                if (mpTeam != null && singlePlayerTeam != null)
                {
                    mpTeam.CopyFrom(singlePlayerTeam, maxTeamSize);
                }
                teamManager.SetCurrentCharacterIndex(0);
            }
            
            if (player != Host)
            {
                // Broadcast chat notification for player entering world
                BroadcastPacket(new PacketPlayerChatNotify(player, 0, 1)); // SYSTEM_HINT_TYPE_CHAT_ENTER_WORLD = 1
            }
        }
        
        // Set scene ID if specified
        if (newSceneId != -1)
        {
            player.SceneId = (uint)newSceneId;
        }
        
        // Add to scene
        var scene = GetSceneById((int)player.SceneId);
        if (scene != null)
        {
            scene.AddPlayer(player);
        }
        
        // Info packet for other players
        if (_players.Count > 1)
        {
            UpdatePlayerInfos(player);
        }
    }
    
    private void UpdatePlayerInfos(PlayerInstance newPlayer)
    {
        // Update player infos for other players (simplified version)
        foreach (var player in _players)
        {
            // Don't send packets to the joining player
            if (player == newPlayer)
                continue;
            
            // TODO: Implement proper packet sending when packet classes are available
            // In Java version, the following packets are sent:
            // player.GetSession().Send(new PacketWorldPlayerInfoNotify(this));
            // player.GetSession().Send(new PacketScenePlayerInfoNotify(this));
            // player.GetSession().Send(new PacketWorldPlayerRTTNotify(this));
            // player.GetSession().Send(new PacketSyncTeamEntityNotify(player));
            // player.GetSession().Send(new PacketSyncScenePlayTeamEntityNotify(player));
            
            // For now, we just log the update
            Logger.Info($"UpdatePlayerInfos: Notifying player {player.Uid} about new player {newPlayer.Uid}");
        }
    }
    
    /// <summary>
    /// Adds a player to this world. Removes from previous world if necessary.
    /// Fixed to prevent deadlock by removing player from old world BEFORE acquiring lock.
    /// </summary>
    /// <param name="player">The player to add to this world.</param>
    /// <exception cref="ArgumentNullException">Thrown when player is null.</exception>
    public void AddPlayer(PlayerInstance player)
    {
        // Parameter validation
        if (player == null)
            throw new ArgumentNullException(nameof(player), "Player cannot be null");
        
        // Check if player already in
        if (_players.Contains(player))
            return;
        
        // Remove from previous world if necessary
        if (player.World != null && player.World != this)
        {
            player.World.RemovePlayer(player);
        }
        
        // Now add to this world
        AddPlayerInternal(player, -1);
    }
    
    /// <summary>
    /// Adds a player to this world with a specific scene ID.
    /// Fixed to prevent deadlock by removing player from old world BEFORE acquiring lock.
    /// </summary>
    /// <param name="player">The player to add to this world.</param>
    /// <param name="newSceneId">Optional scene ID (use -1 to use player's current scene).</param>
    /// <exception cref="ArgumentNullException">Thrown when player is null.</exception>
    public void AddPlayer(PlayerInstance player, int newSceneId)
    {
        // Parameter validation
        if (player == null)
            throw new ArgumentNullException(nameof(player), "Player cannot be null");
        
        // Check if player already in
        if (_players.Contains(player))
            return;
        
        // Remove from previous world if necessary
        if (player.World != null && player.World != this)
        {
            player.World.RemovePlayer(player);
        }
        
        // Now add to this world
        AddPlayerInternal(player, newSceneId);
    }
    
    /// <summary>
    /// Removes a player from this world.
    /// Fixed to prevent deadlock by using fine-grained locking and not holding locks
    /// when calling addPlayer on other worlds.
    /// </summary>
    /// <param name="player">The player to remove from this world.</param>
    /// <exception cref="ArgumentNullException">Thrown when player is null.</exception>
    public void RemovePlayer(PlayerInstance player)
    {
        // Parameter validation
        if (player == null)
            throw new ArgumentNullException(nameof(player), "Player cannot be null");
        
        // Prepare data before acquiring lock
        // Note: In Java version, there's additional logic for team entity IDs and kicking players
        // We'll implement the basic removal for now
        
        // Critical section with fine-grained lock
        lock (_playerListLock)
        {
            if (!_players.Contains(player))
                return;
            
            _players.Remove(player);
            player.World = null;
        }
        
        // Remove from scene (outside lock to prevent circular dependency)
        var scene = GetSceneById((int)player.SceneId);
        scene?.RemovePlayer(player);
        
        // If host leaves, handle multiplayer transition
        if (player == Host && _players.Count > 0)
        {
            // TODO: Transfer host
        }
        
        // Broadcast chat notification for player leaving world
        if (_players.Count > 0) // Only broadcast if there are other players left
        {
            BroadcastPacket(new PacketPlayerChatNotify(player, 0, 2)); // SYSTEM_HINT_TYPE_CHAT_LEAVE_WORLD = 2
        }
    }
    
    public uint GetNextPeerId() => ++_nextPeerId;
    
    /// <summary>
    /// Broadcasts a packet to all players in this world.
    /// </summary>
    /// <param name="packet">The packet to broadcast.</param>
    public void BroadcastPacket(BasePacket packet)
    {
        lock (_playerListLock)
        {
            foreach (var player in _players)
            {
                if (player.Connection?.IsOnline == true)
                {
                    _ = player.SendPacket(packet);
                }
            }
        }
    }
    
    public PlayerInstance? GetHost() => Host;
    
    public int GetNextEntityId(EntityIdTypeEnum idType)
    {
        return ((int)idType << 22) + ++_nextEntityId;
    }

    public uint getLevelEntityId()
    {
        return Entity.Id;
    }
}
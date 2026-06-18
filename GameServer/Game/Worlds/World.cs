using NahidaImpact.Util;
using NahidaImpact.Data;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.GameServer.Game.Player;
using System.Collections.Concurrent;
using NahidaImpact.Enums.Entity;
using NahidaImpact.KcpSharp;
using NahidaImpact.GameServer.Server.Packet.Send.Player;

namespace NahidaImpact.GameServer.Game.Worlds;

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
    
    private void AddPlayerInternal(PlayerInstance player, int newSceneId)
    {
        lock (_playerListLock)
        {
            if (_players.Contains(player))
                return;
            
            player.World = this;
            _players.Add(player);
        }
        
        player.PeerId = GetNextPeerId();
        
        player.TeamManager?.SetEntity(new EntityTeam(player.Scene));
        
        if (_isMultiplayer)
        {
            var teamManager = player.TeamManager;
            if (teamManager != null)
            {
                var mpTeam = teamManager.MpTeam;
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
                BroadcastPacket(new PacketPlayerChatNotify(player, 0, new ChatInfo.Types.SystemHint { Type = 1 })); // SYSTEM_HINT_TYPE_CHAT_ENTER_WORLD
            }
        }
        
        if (newSceneId != -1)
        {
            player.SceneId = (uint)newSceneId;
        }
        
        var scene = GetSceneById((int)player.SceneId);
        if (scene != null)
        {
            scene.AddPlayer(player);
        }
        
        if (_players.Count > 1)
        {
            UpdatePlayerInfos(player);
        }
    }
    
    private void UpdatePlayerInfos(PlayerInstance newPlayer)
    {
        foreach (var player in _players)
        {
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
    
    public void AddPlayer(PlayerInstance player)
    {
        if (player == null) 
            throw new ArgumentNullException(nameof(player), "Player cannot be null");
        if (_players.Contains(player)) return;
        if (player.World != null && player.World != this) player.World.RemovePlayer(player);
        AddPlayerInternal(player, -1);
    }
    
    public void AddPlayer(PlayerInstance player, int newSceneId)
    {
        if (player == null)
            throw new ArgumentNullException(nameof(player), "Player cannot be null");
        if (_players.Contains(player))
            return;
        if (player.World != null && player.World != this) player.World.RemovePlayer(player);
        AddPlayerInternal(player, newSceneId);
    }
    
    public void RemovePlayer(PlayerInstance player)
    {
        if (player == null)
            throw new ArgumentNullException(nameof(player), "Player cannot be null");
        
        lock (_playerListLock)
        {
            if (!_players.Contains(player))
                return;
            
            _players.Remove(player);
            player.World = null;
        }
        
        var scene = GetSceneById((int)player.SceneId);
        scene?.RemovePlayer(player);

        if (player == Host && _players.Count > 0)
        {
            // TODO: Transfer host
        }
        
        // Broadcast chat notification for player leaving world
        if (_players.Count > 0) // Only broadcast if there are other players left
        {
            BroadcastPacket(new PacketPlayerChatNotify(player, 0, new ChatInfo.Types.SystemHint { Type = 2 })); // SYSTEM_HINT_TYPE_CHAT_LEAVE_WORLD
        }
    }
    
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

    public List<PlayerInstance> GetPlayers()
    {
        lock (_playerListLock) return _players.ToList();
    }

    public int GetPlayerCount()
    {
        lock (_playerListLock) return _players.Count;
    }

    public bool IsMultiplayer() => _isMultiplayer;

    public uint GetHostPeerId() => Host?.PeerId ?? 0;

    public int GetNextEntityId(EntityIdTypeEnum idType) => ((int)idType << 21) + ++_nextEntityId;

    public uint getLevelEntityId() => Entity.Id;

    public uint GetNextPeerId() => ++_nextPeerId;
    
    public bool TransferPlayerToScene(PlayerInstance player, int targetSceneId, Position targetPos)
    {
        if (player == null)
            return false;

        var oldScene = player.Scene;
        var newScene = GetSceneById(targetSceneId);
        if (newScene == null)
            return false;

        // Remove from old scene
        oldScene?.RemovePlayer(player);

        // Update player position and scene
        player.Position.Set(targetPos);
        player.SceneId = (uint)targetSceneId;
        player.PrevScene = oldScene?.Id ?? 3;
        player.SetPrevPos(targetPos.Clone());

        // Add to new scene
        newScene.AddPlayer(player);

        return true;
    }
}
using NahidaImpact.Util;
using NahidaImpact.Data;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.Internationalization;
using System.Collections.Concurrent;
using NahidaImpact.Enums.Entity;
using NahidaImpact.Enums.Player;
using NahidaImpact.Enums.Scene;
using NahidaImpact.KcpSharp;
using NahidaImpact.GameServer.Server.Packet.Send.Player;
using NahidaImpact.GameServer.Server.Packet.Send.Scene;

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
        
        if (player.Scene != null)
        {
            player.TeamManager?.SetEntity(new EntityTeam(player.Scene));
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
            Logger.Info(I18NManager.Translate("Game.WorldInfo.NotifyPlayerInfo", player.Uid.ToString(), newPlayer.Uid.ToString()));
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
    
    // Simple teleport to a scene/position.
    public bool TransferPlayerToScene(PlayerInstance player, int sceneId, Position pos)
    {
        return TransferPlayerToScene(player, sceneId, TeleportType.Internal, pos);
    }

    // Teleport with an explicit type (WAYPOINT, COMMAND, etc.).
    public bool TransferPlayerToScene(PlayerInstance player, int sceneId, TeleportType teleportType, Position pos)
    {
        var enterReason = teleportType switch
        {
            TeleportType.Internal => EnterReason.TransPoint,
            TeleportType.Waypoint => EnterReason.TransPoint,
            TeleportType.Map => EnterReason.TransPoint,
            TeleportType.Command => EnterReason.Gm,
            TeleportType.Script => EnterReason.Lua,
            TeleportType.Client => EnterReason.ClientTransmit,
            TeleportType.Dungeon => EnterReason.DungeonEnter,
            _ => EnterReason.None
        };
        return TransferPlayerToScene(player, sceneId, teleportType, enterReason, pos);
    }

    // Teleport with explicit teleport type and enter reason.
    public bool TransferPlayerToScene(PlayerInstance player, int sceneId, TeleportType teleportType,
        EnterReason enterReason, Position teleportTo)
    {
        var props = new TeleportProperties
        {
            SceneId = sceneId,
            TeleportType = teleportType,
            EnterReason = enterReason,
            TeleportTo = teleportTo,
            EnterType = EnterType.Jump
        };

        // Resolve EnterType: GOTO for same scene, DUNGEON handled separately, HOME via scene type.
        if (player.SceneId == sceneId)
        {
            props.EnterType = EnterType.Goto;
        }
        else if (GameData.SceneData.TryGetValue(sceneId, out var sceneData)
                 && sceneData.SceneType == SceneTypeEnum.SceneHomeWorld)
        {
            props.EnterType = EnterType.SelfHome;
            props.EnterReason = EnterReason.EnterHome;
        }

        return TransferPlayerToScene(player, props);
    }

    // Core teleport engine with full TeleportProperties.
    public bool TransferPlayerToScene(PlayerInstance player, TeleportProperties props)
    {
        if (player == null)
            return false;

        // Validate destination scene exists in game data.
        if (!GameData.SceneData.ContainsKey(props.SceneId))
        {
            Logger.Warn($"Teleport to unknown scene {props.SceneId}");
            return false;
        }

        // Default target position to current position if not specified.
        props.TeleportTo ??= player.Position.Clone();

        // Save previous scene/position BEFORE the transition for the enter-scene packet.
        var oldScene = player.Scene;
        int prevSceneId = (int)player.SceneId;
        var prevPos = player.Position.Clone();

        var newScene = GetSceneById(props.SceneId);
        if (newScene == null)
            return false;

        // Same-scene fast path: just move the player, no full scene transition.
        if (newScene == oldScene && props.TeleportType == TeleportType.Command)
        {
            if (props.TeleportTo != null)
                player.Position.Set(props.TeleportTo);
            if (props.TeleportRot != null)
                player.Rotation.Set(props.TeleportRot);
            _ = player.SendPacket(new PacketSceneEntityAppearNotify(player));
            return true;
        }

        // Remove from old scene (preserve scene if teleporting back into it).
        if (oldScene != null)
        {
            if (oldScene == newScene)
                oldScene.SetDontDestroyWhenEmpty(true);
            oldScene.RemovePlayer(player);
        }

        // Add to new scene.
        if (newScene != null)
        {
            newScene.AddPlayer(player);

            // Resolve fallback position from scene config if not provided.
            // TODO: Use scene script config born_pos/born_rot when available.
        }

        // Update player position and rotation.
        if (props.TeleportTo != null)
            player.Position.Set(props.TeleportTo);
        if (props.TeleportRot != null)
            player.Rotation.Set(props.TeleportRot);

        // Track previous scene for cross-scene transitions.
        if (oldScene != null && newScene != null && newScene != oldScene)
        {
            newScene.SetPrevScenePoint(oldScene.GetPrevScenePoint());
            oldScene.SetDontDestroyWhenEmpty(false);
        }

        player.PrevScene = prevSceneId;
        player.SetPrevPos(prevPos);

        // Send enter-scene notify with saved previous scene/position.
        _ = player.SendPacket(new PacketPlayerEnterSceneNotify(player, props, prevSceneId, prevPos));

        return true;
    }
}
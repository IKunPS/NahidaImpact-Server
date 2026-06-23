using NahidaImpact.Data;
using NahidaImpact.Enums.Entity;
using NahidaImpact.Enums.Player;
using NahidaImpact.Enums.Scene;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.GameServer.Server.Packet.Send.Player;
using NahidaImpact.GameServer.Server.Packet.Send.Scene;
using NahidaImpact.KcpSharp;
using NahidaImpact.Util;
using System.Collections.Concurrent;

namespace NahidaImpact.GameServer.Game.Worlds;

// hk4e World → PlayerWorld — manages scenes, players, and world-level state
public class World
{
    private static readonly Logger Logger = new("World");

    public PlayerInstance Host { get; private set; }
    public EntityWorld Entity { get; private set; }

    private readonly List<PlayerInstance> _players = [];
    private readonly ConcurrentDictionary<int, Scene> _scenes = [];
    private readonly object _playerLock = new();

    private int _nextEntityId;
    private uint _nextPeerId;
    private int _worldLevel;
    private bool _isMultiplayer;

    // hk4e PlayerWorld fields
    public int WorldLevel
    {
        get => _worldLevel;
        set { _worldLevel = value; LastAdjustTime = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds(); }
    }
    public uint LastAdjustTime { get; private set; }
    public int AdjustLevel { get; set; }

    public bool IsMultiplayer => _isMultiplayer;
    public uint HostPeerId => Host?.PeerId ?? 0;
    public uint LevelEntityId => Entity.Id;
    public uint NextPeerId => ++_nextPeerId;

    public World(PlayerInstance host, bool isMultiplayer = false)
    {
        Host = host;
        _worldLevel = host.Data.WorldLevel;
        _isMultiplayer = isMultiplayer;
        Entity = new EntityWorld(this);
    }

    #region Scene Management

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

    public void RegisterScene(Scene scene) => _scenes[scene.Id] = scene;
    public void DeregisterScene(Scene scene) => _scenes.TryRemove(scene.Id, out _);

    #endregion

    #region Player Management

    public List<PlayerInstance> Players
    {
        get { lock (_playerLock) return _players.ToList(); }
    }

    public int PlayerCount
    {
        get { lock (_playerLock) return _players.Count; }
    }

    public void AddPlayer(PlayerInstance player)
    {
        if (player == null) throw new ArgumentNullException(nameof(player));
        if (Players.Contains(player)) return;
        if (player.World != null && player.World != this) player.World.RemovePlayer(player);
        AddPlayerInternal(player, -1);
    }

    public void AddPlayer(PlayerInstance player, int newSceneId)
    {
        if (player == null) throw new ArgumentNullException(nameof(player));
        if (Players.Contains(player)) return;
        if (player.World != null && player.World != this) player.World.RemovePlayer(player);
        AddPlayerInternal(player, newSceneId);
    }

    private void AddPlayerInternal(PlayerInstance player, int newSceneId)
    {
        lock (_playerLock)
        {
            if (_players.Contains(player)) return;
            player.World = this;
            _players.Add(player);
        }

        player.PeerId = NextPeerId;

        if (_isMultiplayer)
        {
            var tm = player.TeamManager;
            if (tm != null)
            {
                tm.MpTeam.CopyFrom(tm.CurrentSinglePlayerTeamInfo, tm.MaxTeamSize);
                tm.SetCurrentCharacterIndex(0);
            }

            if (player != Host)
            {
                BroadcastPacket(new PacketPlayerChatNotify(player, 0,
                    new ChatInfo.Types.SystemHint { Type = 1 }));
            }
        }

        if (newSceneId != -1)
            player.SceneId = (uint)newSceneId;

        var scene = GetSceneById((int)player.SceneId);
        scene?.AddPlayer(player);

        if (player.Scene != null && player.TeamManager != null)
            player.TeamManager.Entity = new EntityTeam(player.Scene);

        if (_players.Count > 1)
            UpdatePlayerInfos(player);
    }

    public void RemovePlayer(PlayerInstance player)
    {
        if (player == null) throw new ArgumentNullException(nameof(player));

        lock (_playerLock)
        {
            if (!_players.Contains(player)) return;
            _players.Remove(player);
            player.World = null;
        }

        GetSceneById((int)player.SceneId)?.RemovePlayer(player);

        if (player == Host && _players.Count > 0)
        {
            // TODO: transfer host to next player
            Host = _players[0];
        }

        if (_players.Count > 0)
        {
            BroadcastPacket(new PacketPlayerChatNotify(player, 0,
                new ChatInfo.Types.SystemHint { Type = 2 }));
        }
    }

    private void UpdatePlayerInfos(PlayerInstance newPlayer)
    {
        var notify = new PacketWorldPlayerInfoNotify(newPlayer);
        foreach (var player in _players)
        {
            if (player == newPlayer) continue;
            _ = player.SendPacket(notify);
        }
    }

    // hk4e PlayerWorld::transferHost — transfers ownership when host leaves
    public void TransferHost(PlayerInstance nextHost)
    {
        if (nextHost == Host) return;
        Host = nextHost;
        // Per-player worlds don't need full host transfer; in true MP,
        // PacketWorldPlayerInfoNotify + PacketScenePlayerInfoNotify would broadcast
    }

    #endregion

    #region Teleport

    public bool TransferPlayerToScene(PlayerInstance player, int sceneId, Position pos)
        => TransferPlayerToScene(player, sceneId, TeleportType.Internal, pos);

    public bool TransferPlayerToScene(PlayerInstance player, int sceneId, TeleportType type, Position pos)
    {
        var enterReason = type switch
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
        return TransferPlayerToScene(player, sceneId, type, enterReason, pos);
    }

    public bool TransferPlayerToScene(PlayerInstance player, int sceneId, TeleportType type,
        EnterReason reason, Position pos)
    {
        var props = new TeleportProperties
        {
            SceneId = sceneId,
            TeleportType = type,
            EnterReason = reason,
            TeleportTo = pos,
            EnterType = EnterType.Jump
        };

        if (player.SceneId == sceneId)
            props.EnterType = EnterType.Goto;
        else if (GameData.SceneData.TryGetValue(sceneId, out var sd)
                 && sd.SceneType == SceneTypeEnum.SCENE_HOME_WORLD)
        {
            props.EnterType = EnterType.SelfHome;
            props.EnterReason = EnterReason.EnterHome;
        }

        return TransferPlayerToScene(player, props);
    }

    public bool TransferPlayerToScene(PlayerInstance player, TeleportProperties props)
    {
        if (player == null) return false;
        if (!GameData.SceneData.ContainsKey(props.SceneId)) return false;

        props.TeleportTo ??= player.Position.Clone();

        var oldScene = player.Scene;
        int prevSceneId = (int)player.SceneId;
        var prevPos = player.Position.Clone();

        var newScene = GetSceneById(props.SceneId);
        if (newScene == null) return false;

        // Same-scene fast path
        if (newScene == oldScene && props.TeleportType == TeleportType.Command)
        {
            if (props.TeleportTo != null) player.Position.Set(props.TeleportTo);
            if (props.TeleportRot != null) player.Rotation.Set(props.TeleportRot);
            _ = player.SendPacket(new PacketSceneEntityAppearNotify(player));
            return true;
        }

        if (oldScene != null)
        {
            if (oldScene == newScene) oldScene.DontDestroyWhenEmpty = true;
            oldScene.RemovePlayer(player);
        }

        newScene.AddPlayer(player);

        if (props.TeleportTo != null) player.Position.Set(props.TeleportTo);
        if (props.TeleportRot != null) player.Rotation.Set(props.TeleportRot);

        if (oldScene != null && newScene != oldScene)
        {
            newScene.PrevScenePoint = oldScene.PrevScenePoint;
            oldScene.DontDestroyWhenEmpty = false;
        }

        player.PrevScene = prevSceneId;
        player.SetPrevPos(prevPos);

        _ = player.SendPacket(new PacketPlayerEnterSceneNotify(player, props, prevSceneId, prevPos));
        return true;
    }

    #endregion

    #region Entity / Broadcast

    // hk4e World::getNextEntityId — entity ID = (type << 21) + counter
    public int GetNextEntityId(EntityIdTypeEnum type) => ((int)type << 21) + ++_nextEntityId;

    public void BroadcastPacket(BasePacket packet)
    {
        List<PlayerInstance> snapshot;
        lock (_playerLock) snapshot = _players.ToList();
        foreach (var player in snapshot)
        {
            if (player.Connection?.IsOnline == true)
                _ = player.SendPacket(packet);
        }
    }

    // hk4e PlayerWorld::genRewardPointKey
    public static ulong GenRewardPointKey(uint groupId, uint configId)
        => ((ulong)groupId << 32) + configId;

    #endregion
}

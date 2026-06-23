using System;
using NahidaImpact.Data;
using NahidaImpact.Database;
using NahidaImpact.Database.Account;
using NahidaImpact.Database.Avatar;
using NahidaImpact.Database.Inventory;
using NahidaImpact.Database.Player;
using NahidaImpact.GameServer.Game.Avatar;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.GameServer.Game.Friends;
using NahidaImpact.GameServer.Game.Inventory;
using NahidaImpact.GameServer.Game.Ability;
using NahidaImpact.GameServer.Game.MapMarks;
using NahidaImpact.GameServer.Game.Player.Team;
using NahidaImpact.GameServer.Game.Worlds;
using NahidaImpact.GameServer.Server;
using NahidaImpact.GameServer.Server.Packet.Send.Avatar;
using NahidaImpact.GameServer.Server.Packet.Send.Inventory;
using NahidaImpact.GameServer.Server.Packet.Send.Player;
using NahidaImpact.GameServer.Server.Packet.Send.State;
using NahidaImpact.GameServer.Server.Packet.Send.Map;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;
using NahidaImpact.Enums.Player;
using NahidaImpact.Util.Extensions;
using NahidaImpact.Util;

namespace NahidaImpact.GameServer.Game.Player;

public class PlayerInstance
{
    public AvatarManager AvatarManager { get; private set; }
    public InventoryManager InventoryManager { get; private set; }
    public WeaponManager WeaponManager { get; private set; }
    public static readonly List<PlayerInstance> PlayerInstances = [];
    public PlayerData Data { get; set; }
    public EntityAvatar? EntityAvatar { get; set; }
    public List<AvatarDataInfo> Avatars => AvatarManager.AvatarData.Avatars;
    public SocialManager SocialManager { get; private set; }
    public PlayerProfile Profile { get; private set; }
    public ProgressManager ProgressManager { get; private set; }
    public AbilityManager AbilityManager { get; private set; }
    public CombatInvokeHandler CombatInvokeHandler { get; private set; }
    public TeamManager TeamManager { get; private set; }
    public MapMarksManager MapMarksManager { get; private set; }
    public Scene Scene { get; internal set; }
    public World World { get; internal set; }
    
    public uint SceneId { get; set; } = GameConstants.START_SCENE_ID;
    public int Uid { get; set; }
    public uint PeerId { get; internal set; }
    public Connection? Connection { get; set; }
    public bool IsNewPlayer { get; set; }
    public bool HasSentLoginPackets { get; set; }
    public uint GuidSeed { get; set; }
    public ulong GetNextGameGuid() => ((ulong)Uid << 32) + (++GuidSeed);
    public int Primogems { get; set; }
    public int Mora { get; set; }
    public int Crystals { get; set; }
    public int HomeCoin { get; set; }
    public uint EnterToken { get; set; }
    public SceneLoadState SceneLoadState { get; set; } = SceneLoadState.None;

    /// <summary>
    /// When true, inventory/avatar mutations skip their per-item notify packets and
    /// per-call database saves. Callers must flush a full snapshot (PlayerStoreNotify +
    /// AvatarDataNotify) and Save() once after the batch completes. Used by give-all
    /// so the client rebuilds everything from one notify instead of streaming 600+.
    /// </summary>
    public bool SuppressNotifications { get; set; }

    /// <summary>True only for the very first enter-scene after account creation.</summary>
    public bool IsFirstLoginEnterScene => SceneLoadState == SceneLoadState.None;
    public int MainCharacterId { get; set; }
    public List<uint> ChatEmojiIdList { get; set; } = [];
    public List<int> FlyCloakList { get; set; } = [GameConstants.DEFAULT_FLYCLOAK_ID];
    public List<int> NameCardList { get; set; } = [GameConstants.DEFAULT_NAME_CARD_ID];

    public uint WeaponEntityId
    {
        get
        {
            var avatar = TeamManager?.GetCurrentAvatarEntity();
            if (avatar != null)
            {
                var weapon = InventoryManager.Items.Values
                    .FirstOrDefault(i => i.EquipCharacter == (int)avatar.AvatarInfo.AvatarId
                        && i.ItemType == Enums.Item.ItemType.ITEM_WEAPON);
                if (weapon != null && weapon.WeaponEntityId > 0)
                    return (uint)weapon.WeaponEntityId;
            }
            return 0x06000064;
        }
    }

    public Position Position { get; private set; } = new(GameConstants.START_POS_X, GameConstants.START_POS_Y, GameConstants.START_POS_Z);
    public Position Rotation { get; private set; } = new(GameConstants.START_ROT_X, GameConstants.START_ROT_Y, GameConstants.START_ROT_Z);
    public Position PrevPos { get; private set; } = new();
    public Position PrevPosForHome { get; private set; } = Position.Zero;
    public int PrevScene { get; set; }
    
    public Dictionary<int, HashSet<int>> UnlockedScenePoints { get; set; } = [];
    public Dictionary<int, HashSet<int>> UnlockedSceneAreas { get; set; } = [];
    public Dictionary<string, MapMark> MapMarks { get; set; } = [];
    public Dictionary<int, HashSet<int>> SceneTags { get; set; } = [];
    
    public int AreaId { get; set; }
    public int AreaType { get; set; }
    public Dictionary<int, MapAreaInfo> MapAreas { get; set; } = [];

    public HashSet<int> GetUnlockedScenePoints(int sceneId)
    {
        if (!UnlockedScenePoints.TryGetValue(sceneId, out var points))
        {
            points = [];
            UnlockedScenePoints[sceneId] = points;
        }
        return points;
    }

    public HashSet<int> GetUnlockedSceneAreas(int sceneId)
    {
        if (!UnlockedSceneAreas.TryGetValue(sceneId, out var areas))
        {
            areas = [];
            UnlockedSceneAreas[sceneId] = areas;
        }
        return areas;
    }
    
    public void ApplyStartingSceneTags()
    {
        foreach (var sceneTag in GameData.SceneTagData.Values)
        {
            if (!sceneTag.IsDefaultValid) continue;

            if (!SceneTags.TryGetValue(sceneTag.SceneId, out var tags))
            {
                tags = [];
                SceneTags[sceneTag.SceneId] = tags;
            }
            tags.Add((int)sceneTag.Id);
        }
    }
    
    public PlayerInstance(PlayerData data)
    {
        Data = data;
        Uid = data.Uid;
        Profile = new PlayerProfile(data.Name ?? "Traveler");

        TeamManager = new TeamManager(this);
        AvatarManager = new AvatarManager(this);
        InventoryManager = new InventoryManager(this);
        WeaponManager = new WeaponManager(this);
        SocialManager = new SocialManager(this);
        ProgressManager = new ProgressManager(this);
        AbilityManager = new AbilityManager(this);
        CombatInvokeHandler = new CombatInvokeHandler(this);
        MapMarksManager = new MapMarksManager(this);

        ApplyProperties();
    }

    private void ApplyProperties()
    {
        SetProperty(Prop.PlayerProp.PROP_PLAYER_LEVEL, GameConstants.DEFAULT_PLAYER_LEVEL);
        SetProperty(Prop.PlayerProp.PROP_IS_SPRING_AUTO_USE, 1);
        SetProperty(Prop.PlayerProp.PROP_SPRING_AUTO_USE_PERCENT, 50);
        SetProperty(Prop.PlayerProp.PROP_IS_FLYABLE, 1);
        SetProperty(Prop.PlayerProp.PROP_PLAYER_CAN_DIVE, 1);
        SetProperty(Prop.PlayerProp.PROP_IS_TRANSFERABLE, 1);
        SetProperty(Prop.PlayerProp.PROP_MAX_STAMINA, GameConstants.MAX_STAMINA_DEFAULT);
        SetProperty(Prop.PlayerProp.PROP_DIVE_MAX_STAMINA, GameConstants.DIVE_MAX_STAMINA_DEFAULT);
        SetProperty(Prop.PlayerProp.PROP_PLAYER_RESIN, GameConstants.PLAYER_RESIN_DEFAULT);
        SetProperty(Prop.PlayerProp.PROP_PHLOGISTON_ENABLE, 1);
        SetProperty(Prop.PlayerProp.PROP_CUR_PERSIST_STAMINA, GameConstants.MAX_STAMINA_DEFAULT);
        SetProperty(Prop.PlayerProp.PROP_DIVE_CUR_STAMINA, GameConstants.DIVE_MAX_STAMINA_DEFAULT);
        SetProperty(Prop.PlayerProp.PROP_PLAYER_MP_SETTING_TYPE, GameConstants.MP_SETTING_NONE);
        SetProperty(Prop.PlayerProp.PROP_IS_MP_MODE_AVAILABLE, 1);
    }

    public void SetPosition(Position position) => Position.Set(position);
    public void SetRotation(Position rotation) => Rotation.Set(rotation);
    public void SetPrevPos(Position pos) => PrevPos.Set(pos);
    public void SetPrevPosForHome(Position pos) => PrevPosForHome.Set(pos);

    #region Initializers

    public PlayerInstance(int uid) : this(new PlayerData { Uid = uid })
    {
        IsNewPlayer = true;
        Data.Name = AccountData.GetAccountByUid(uid)?.Username;
        Profile = new PlayerProfile(Data.Name ?? "Traveler");
        DatabaseHelper.CreateInstance(Data);
    }

    public T InitializeDatabase<T>() where T : BaseDatabaseDataHelper, new()
    {
        var instance = DatabaseHelper.GetInstanceOrCreateNew<T>(Uid);
        return instance!;
    }

    #endregion

    #region Network

    public async ValueTask OnLogin()
    {
        PlayerInstances.Add(this);

        Data.LastActiveTime = Extensions.UnixSec;
        Profile.LastActiveTime = Data.LastActiveTime;

        World = new World(this);
        World.AddPlayer(this);

        ProgressManager.OnPlayerLogin();
        ApplyStartingSceneTags();
        await SendPacket(new PacketPlayerWorldSceneInfoListNotify(this));

        await SendPacket(new PacketPlayerEnterSceneNotify(this));
        await SendPacket(new PacketPlayerDataNotify(this));
        HasSentLoginPackets = true;

        InventoryManager.LoadFromDatabase();
        await SendPacket(new PacketPlayerStoreNotify(InventoryManager.Data.Items));
        await SendPacket(new PacketAvatarDataNotify(this));
        await SendPacket(new PacketOpenStateUpdateNotify(this));
    }

    public static PlayerInstance? GetPlayerInstanceByUid(uint uid)
        => PlayerInstances.FirstOrDefault(player => player.Uid == uid);
    
    public async ValueTask DropMessage(string message)
    {
        await SendPacket(new PacketPrivateChatNotify(GameConstants.SERVER_CONSOLE_UID, Uid, message));
    }

    public void OnLogoutAsync()
    {
        Chat.ChatSystem.Instance.ClearHistoryOnLogout(this);
        PlayerInstances.Remove(this);
    }
    public async ValueTask SendPacket(BasePacket packet)
    {
        if (Connection?.IsOnline == true)
        {
            await Connection.SendPacket(packet);
        }
        else
        {
            Logger.GetByClassName().Warn(
                $"SendPacket: dropped CmdID={packet.CmdId}, Connection.IsOnline={Connection?.IsOnline}");
        }
    }

    #endregion

    #region Map Area Tracking
    
    public void AddMapArea(int mapAreaId, bool isOpen)
    {
        MapAreas[mapAreaId] = new MapAreaInfo
        {
            MapAreaId = (uint)mapAreaId,
            IsOpen = isOpen
        };
    }
    
    public void RemoveMapArea(int mapAreaId)
    {
        MapAreas.Remove(mapAreaId);
    }

    public void SetMapAreaOpen(int mapAreaId, bool isOpen)
    {
        if (MapAreas.TryGetValue(mapAreaId, out var area))
        {
            area.IsOpen = isOpen;
        }
    }
    
    public void SetArea(int areaId, int areaType)
    {
        AreaId = areaId;
        AreaType = areaType;
    }

    #endregion

    #region Helpers
    public bool IsInMultiplayer()
    {
        // TODO: Implement multiplayer check
        return false;
    }

    private float _phlogistonValue = 100f;

    public bool IsUnlimitedPhlogiston()
    {
        return false;
    }

    public float PhlogistonValue
    {
        get => _phlogistonValue;
        set => _phlogistonValue = System.Math.Max(0, System.Math.Min(100, value));
    }

    /// <summary>Player property map (MAX_STAMINA, CUR_PERSIST_STAMINA, etc.)</summary>
    private readonly Dictionary<uint, int> _playerProperties = new();
    public IReadOnlyDictionary<uint, int> Properties => _playerProperties;

    public int GetProperty(uint propType)
    {
        return _playerProperties.TryGetValue(propType, out int value) ? value : 0;
    }

    public void SetProperty(uint propType, int value)
    {
        _playerProperties[propType] = value;
        // TODO: Send PacketPlayerPropNotify to client
    }

    private List<int> _costumeList = [];
    public List<int> CostumeList => _costumeList;

    private List<int> _traceEffectList = [];
    public List<int> TraceEffectList => _traceEffectList;

    public int GetMainCharacterId()
    {
        // If set explicitly (via born data), return it
        if (MainCharacterId != 0)
            return MainCharacterId;

        // Check if we have a male or female main character
        var avatars = AvatarManager.AvatarData.Avatars;
        foreach (var a in avatars)
        {
            if (a.AvatarId == GameConstants.MAIN_CHARACTER_MALE ||
                a.AvatarId == GameConstants.MAIN_CHARACTER_FEMALE)
                return (int)a.AvatarId;
        }
        return (int)(avatars.FirstOrDefault()?.AvatarId ?? (uint)GameConstants.MAIN_CHARACTER_FEMALE);
    }

    #endregion

    #region Avatar

    public async ValueTask<AvatarDataInfo?> AddAvatar(int avatarId, bool addToCurrentTeam = true)
        => await AvatarManager.CreateAvatar(avatarId, addToCurrentTeam);

    /// <summary>Create main character, add to team 1, and trigger full login.</summary>
    public async ValueTask CompleteFirstLogin(int avatarId, int skillDepot, string? nickname = null)
    {
        if (nickname != null)
        {
            Data.Name = nickname;
            Profile.Nickname = nickname;
        }

        var mainChar = await AddAvatar(avatarId, false);
        if (mainChar == null) return;

        mainChar.SkillDepotId = (uint)skillDepot;
        MainCharacterId = avatarId;
        Profile.HeadImage = new ProfilePicture { AvatarId = (uint)avatarId };

        TeamManager.SetMainCharacter(avatarId, mainChar.Guid);
        TeamManager.Save();

        await OnLogin();
    }

    public void OnPlayerBorn()
    {
        // TODO: Trigger born quests via QuestManager
        // player.QuestManager?.OnPlayerBorn();
        // TODO: Send welcome mail with items
    }

    #endregion
    
    public void UnfreezeUnlockedScenePoints()
    {
        // TODO: Implement unfreeze of dungeon scenes' locked points
        // In Java: scenes.stream().filter(Scene::isDungeon).forEach(scene -> scene.unfreezeUnlockedScenePoints(player))
    }

    #region Virtual Item Stubs

    public void AddExpDirectly(int count)
    {
        // TODO: implement adventure rank exp gain
    }

    public void AddResin(int count)
    {
        SetProperty(Prop.PlayerProp.PROP_PLAYER_RESIN,
            GetProperty(Prop.PlayerProp.PROP_PLAYER_RESIN) + count);
    }

    public void UseResin(int count)
    {
        var current = GetProperty(Prop.PlayerProp.PROP_PLAYER_RESIN);
        SetProperty(Prop.PlayerProp.PROP_PLAYER_RESIN, Math.Max(0, current - count));
    }

    public void AddLegendaryKey(int count)
    {
        SetProperty(Prop.PlayerProp.PROP_PLAYER_LEGENDARY_KEY,
            GetProperty(Prop.PlayerProp.PROP_PLAYER_LEGENDARY_KEY) + count);
    }

    public void UseLegendaryKey(int count)
    {
        var current = GetProperty(Prop.PlayerProp.PROP_PLAYER_LEGENDARY_KEY);
        SetProperty(Prop.PlayerProp.PROP_PLAYER_LEGENDARY_KEY, Math.Max(0, current - count));
    }

    public void AddHomeExp(int count)
    {
        // TODO: implement home world exp gain
    }

    #endregion

    #region Actions
    public async ValueTask OnHeartBeat()
    {
        DatabaseHelper.ToSaveUidList.SafeAdd(Uid);
        await Task.CompletedTask;
    }

    #endregion
}
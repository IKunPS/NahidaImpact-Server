using NahidaImpact.Data;
using NahidaImpact.Data.Excel;
using NahidaImpact.Database;
using NahidaImpact.Database.Account;
using NahidaImpact.Database.Avatar;
using NahidaImpact.Database.Player;
using NahidaImpact.Enums.Player;
using NahidaImpact.GameServer.Game.Ability;
using NahidaImpact.GameServer.Game.Avatar;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.GameServer.Game.Friends;
using NahidaImpact.GameServer.Game.Inventory;
using NahidaImpact.GameServer.Game.MapMarks;
using NahidaImpact.GameServer.Game.Player.Team;
using NahidaImpact.GameServer.Game.Worlds;
using NahidaImpact.GameServer.Server;
using NahidaImpact.GameServer.Server.Packet.Send.Avatar;
using NahidaImpact.GameServer.Server.Packet.Send.Inventory;
using NahidaImpact.GameServer.Server.Packet.Send.Map;
using NahidaImpact.GameServer.Server.Packet.Send.Player;
using NahidaImpact.GameServer.Server.Packet.Send.State;
using NahidaImpact.KcpSharp;
using NahidaImpact.Prop;
using NahidaImpact.Util;
using System.Collections.Concurrent;
using NahidaImpact.Util.Extensions;

namespace NahidaImpact.GameServer.Game.Player;

public class PlayerInstance
{
    public static readonly ConcurrentDictionary<int, PlayerInstance> PlayerInstances = new();
    
    // Core managers
    public AvatarManager AvatarManager { get; }
    public InventoryManager InventoryManager { get; }
    public WeaponManager WeaponManager { get; }
    public SocialManager SocialManager { get; }
    public ProgressManager ProgressManager { get; }
    public AbilityManager AbilityManager { get; }
    public CombatInvokeHandler CombatInvokeHandler { get; }
    public TeamManager TeamManager { get; }
    public MapMarksManager MapMarksManager { get; }

    // Data & identity
    public PlayerData Data { get; set; }
    public PlayerProfile Profile { get; set; }
    public List<AvatarDataInfo> Avatars => AvatarManager.AvatarData.Avatars;
    public int Uid { get; set; }
    public uint PeerId { get; internal set; }

    // World & scene
    public Scene? Scene { get; internal set; }
    public World? World { get; internal set; }
    public uint SceneId { get; set; } = 3;
    public bool IsNewPlayer { get; set; }
    public bool HasSentLoginPackets { get; set; }
    public uint EnterToken { get; set; }
    public SceneLoadState SceneLoadState { get; set; } = SceneLoadState.None;

    // Network
    public Connection? Connection { get; set; }

    // Currency
    public int Primogems { get; set; }
    public int Mora { get; set; }
    public int Crystals { get; set; }
    public int HomeCoin { get; set; }

    public uint Rtt { get; set; }
    public int AccountType { get; set; }
    public int PlatformType { get; set; }
    public string ClientIp { get; set; } = "";
    public string Birthday { get; set; } = "";

    // Guid generation
    public uint GuidSeed { get; set; }
    public ulong GetNextGameGuid() => ((ulong)Uid << 32) + (++GuidSeed);

    // Flags
    public bool SuppressNotifications { get; set; }
    public bool IsFirstLoginEnterScene => SceneLoadState == SceneLoadState.None;

    // Entity
    public EntityAvatar? EntityAvatar { get; set; }
    public int MainCharacterId { get; set; }

    public List<uint> ChatEmojiIdList { get => Data.ChatEmojiIdList; set => Data.ChatEmojiIdList = value; }
    public List<int> FlyCloakList { get => Data.FlyCloakList; set => Data.FlyCloakList = value; }
    public List<int> NameCardList { get => Data.NameCardList; set => Data.NameCardList = value; }
    public List<int> CostumeList { get => Data.CostumeList; set => Data.CostumeList = value; }
    public List<int> TraceEffectList { get => Data.TraceEffectList; set => Data.TraceEffectList = value; }
    
    private float _phlogistonValue = 100f;

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

    // Position
    public Position Position { get; private set; } = new(GameConstants.START_POS_X, GameConstants.START_POS_Y, GameConstants.START_POS_Z);
    public Position Rotation { get; private set; } = new(GameConstants.START_ROT_X, GameConstants.START_ROT_Y, GameConstants.START_ROT_Z);
    public Position PrevPos { get; private set; } = new();
    public Position PrevPosForHome { get; private set; } = Position.Zero;
    public int PrevScene { get; set; }

    // Map exploration
    public Dictionary<int, HashSet<int>> UnlockedScenePoints { get; set; } = [];
    public Dictionary<int, HashSet<int>> UnlockedSceneAreas { get; set; } = [];
    public Dictionary<string, MapMark> MapMarks { get; set; } = [];
    public Dictionary<int, HashSet<int>> SceneTags { get; set; } = [];
    public Dictionary<int, MapAreaInfo> MapAreas { get; set; } = [];
    public int AreaId { get; set; }
    public int AreaType { get; set; }

    // Player properties
    private readonly Dictionary<uint, int> _playerProperties = [];
    public IReadOnlyDictionary<uint, int> Properties => _playerProperties;

    #region Constructor

    public PlayerInstance(PlayerData data)
    {
        Data = data;
        Uid = data.Uid;
        Profile = new PlayerProfile(data.Name ?? "Traveler");
        Profile.LoadFromData(data);

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
        ApplyCosmeticDefaults();
    }

    public PlayerInstance(int uid) : this(new PlayerData { Uid = uid })
    {
        IsNewPlayer = true;
        Data.Name = AccountData.GetAccountByUid(uid)?.Username;
        Profile = new PlayerProfile(Data.Name ?? "Traveler");
        DatabaseHelper.CreateInstance(Data);
    }

    private void ApplyProperties()
    {
        SetProperty(PlayerProp.PROP_PLAYER_LEVEL, 1);
        SetProperty(PlayerProp.PROP_IS_SPRING_AUTO_USE, 1);
        SetProperty(PlayerProp.PROP_SPRING_AUTO_USE_PERCENT, 50);
        SetProperty(PlayerProp.PROP_IS_FLYABLE, 1);
        SetProperty(PlayerProp.PROP_PLAYER_CAN_DIVE, 1);
        SetProperty(PlayerProp.PROP_IS_TRANSFERABLE, 1);
        SetProperty(PlayerProp.PROP_MAX_STAMINA, 24000);
        SetProperty(PlayerProp.PROP_DIVE_MAX_STAMINA, 10000);
        SetProperty(PlayerProp.PROP_PLAYER_RESIN, 200);
        SetProperty(PlayerProp.PROP_PHLOGISTON_ENABLE, 1);
        SetProperty(PlayerProp.PROP_CUR_PERSIST_STAMINA, 24000);
        SetProperty(PlayerProp.PROP_DIVE_CUR_STAMINA, 10000);
        SetProperty(PlayerProp.PROP_PLAYER_MP_SETTING_TYPE, 0);
        SetProperty(PlayerProp.PROP_IS_MP_MODE_AVAILABLE, 1);
    }

    private void ApplyCosmeticDefaults()
    {
        var defaultFlycloak = (int)ConstValue.GetUint("CONST_VALUE_DEFAULT_FLYCLOAK_CONFIG");
        var defaultNameCard = (int)ConstValue.GetUint("CONST_VALUE_DEFAULT_NAME_CARD_ID");

        if (FlyCloakList == null) FlyCloakList = [];
        if (NameCardList == null) NameCardList = [];
        if (CostumeList == null) CostumeList = [];
        if (TraceEffectList == null) TraceEffectList = [];
        if (ChatEmojiIdList == null) ChatEmojiIdList = [];

        if (defaultFlycloak > 0 && !FlyCloakList.Contains(defaultFlycloak))
            FlyCloakList.Add(defaultFlycloak);
        if (defaultNameCard > 0 && !NameCardList.Contains(defaultNameCard))
            NameCardList.Add(defaultNameCard);

        // Grant default costumes (hk4e PlayerAvatarComp::tryAddDefaultUnlockCostume)
        foreach (var costume in GameData.CostumeData.Values)
        {
            if (costume.IsDefault && !CostumeList.Contains((int)costume.SkinId))
                CostumeList.Add((int)costume.SkinId);
        }
    }

    #endregion

    #region Properties

    public int GetProperty(uint propType)
        => _playerProperties.TryGetValue(propType, out var value) ? value : 0;

    public void SetProperty(uint propType, int value)
    {
        _playerProperties[propType] = value;
        // TODO: PacketPlayerPropNotify when available
    }

    public float PhlogistonValue
    {
        get => _phlogistonValue;
        set => _phlogistonValue = Math.Max(0, Math.Min(100, value));
    }

    public bool IsFlyable
    {
        get => GetProperty(PlayerProp.PROP_IS_FLYABLE) == 1;
        set => SetProperty(PlayerProp.PROP_IS_FLYABLE, value ? 1 : 0);
    }

    public float CurrentStamina
    {
        get => GetProperty(PlayerProp.PROP_CUR_PERSIST_STAMINA);
        set => SetProperty(PlayerProp.PROP_CUR_PERSIST_STAMINA, (int)Math.Max(0, value));
    }

    public float MaxStamina
    {
        get => GetProperty(PlayerProp.PROP_MAX_STAMINA);
        set => SetProperty(PlayerProp.PROP_MAX_STAMINA, (int)value);
    }

    public float DiveCurrentStamina
    {
        get => GetProperty(PlayerProp.PROP_DIVE_CUR_STAMINA);
        set => SetProperty(PlayerProp.PROP_DIVE_CUR_STAMINA, (int)Math.Max(0, value));
    }

    // Consume stamina and return true if enough was available
    public bool ConsumeStamina(float amount)
    {
        var cur = CurrentStamina;
        if (cur < amount) return false;
        CurrentStamina = cur - amount;
        return true;
    }

    #endregion

    #region Position

    public void SetPosition(Position pos) => Position.Set(pos);
    public void SetRotation(Position rot) => Rotation.Set(rot);
    public void SetPrevPos(Position pos) => PrevPos.Set(pos);
    public void SetPrevPosForHome(Position pos) => PrevPosForHome.Set(pos);

    #endregion

    #region Map

    public HashSet<int> GetUnlockedScenePoints(int sceneId)
    {
        if (!UnlockedScenePoints.TryGetValue(sceneId, out var points))
            UnlockedScenePoints[sceneId] = points = [];
        return points;
    }

    public HashSet<int> GetUnlockedSceneAreas(int sceneId)
    {
        if (!UnlockedSceneAreas.TryGetValue(sceneId, out var areas))
            UnlockedSceneAreas[sceneId] = areas = [];
        return areas;
    }

    public void ApplyStartingSceneTags()
    {
        foreach (var tag in GameData.SceneTagData.Values)
        {
            if (!tag.IsDefaultValid) continue;
            if (!SceneTags.TryGetValue(tag.SceneId, out var tags))
                SceneTags[tag.SceneId] = tags = [];
            tags.Add((int)tag.Id);
        }
    }

    public void AddMapArea(int mapAreaId, bool isOpen)
        => MapAreas[mapAreaId] = new MapAreaInfo { MapAreaId = (uint)mapAreaId, IsOpen = isOpen };

    public void RemoveMapArea(int mapAreaId) => MapAreas.Remove(mapAreaId);

    public void SetMapAreaOpen(int mapAreaId, bool isOpen)
    {
        if (MapAreas.TryGetValue(mapAreaId, out var area))
            area.IsOpen = isOpen;
    }

    public void SetArea(int areaId, int areaType)
    {
        AreaId = areaId;
        AreaType = areaType;
    }

    #endregion

    #region Avatar / Team

    public async ValueTask<AvatarDataInfo?> AddAvatar(int avatarId, bool addToCurrentTeam = true)
        => await AvatarManager.CreateAvatar(avatarId, addToCurrentTeam);

    public async ValueTask CompleteFirstLogin(int avatarId, string? nickname = null)
    {
        if (nickname != null)
        {
            Data.Name = nickname;
            Profile.Nickname = nickname;
        }

        var mainChar = await AddAvatar(avatarId, false);
        if (mainChar == null) return;

        // hk4e: read skillDepotId from AvatarExcelConfigData, not hardcoded constants
        if (GameData.AvatarData.TryGetValue(avatarId, out var avatarExcel))
            mainChar.SkillDepotId = avatarExcel.SkillDepotId;
        MainCharacterId = avatarId;
        Profile.HeadImage = new ProfilePicture { AvatarId = (uint)avatarId };

        TeamManager.SetMainCharacter(avatarId, mainChar.Guid);
        TeamManager.Save();

        await OnLogin();
    }

    public int GetMainCharacterId()
    {
        if (MainCharacterId != 0) return MainCharacterId;

        foreach (var a in Avatars)
        {
            if (a.AvatarId == GameConstants.MAIN_CHARACTER_MALE || a.AvatarId == GameConstants.MAIN_CHARACTER_FEMALE)
                return (int)a.AvatarId;
        }
        return (int)(Avatars.FirstOrDefault()?.AvatarId ?? (uint)GameConstants.MAIN_CHARACTER_FEMALE);
    }

    #endregion

    #region Virtual Items

    public void AddResin(int count)
        => SetProperty(PlayerProp.PROP_PLAYER_RESIN, GetProperty(PlayerProp.PROP_PLAYER_RESIN) + count);

    public void UseResin(int count)
        => SetProperty(PlayerProp.PROP_PLAYER_RESIN, Math.Max(0, GetProperty(PlayerProp.PROP_PLAYER_RESIN) - count));

    public void AddLegendaryKey(int count)
        => SetProperty(PlayerProp.PROP_PLAYER_LEGENDARY_KEY, GetProperty(PlayerProp.PROP_PLAYER_LEGENDARY_KEY) + count);

    public void UseLegendaryKey(int count)
        => SetProperty(PlayerProp.PROP_PLAYER_LEGENDARY_KEY, Math.Max(0, GetProperty(PlayerProp.PROP_PLAYER_LEGENDARY_KEY) - count));

    public void AddExpDirectly(int count)
    {
        // TODO: adventure rank exp gain
    }

    public void AddHomeExp(int count)
    {
        // TODO: home world exp gain
    }

    #endregion

    public bool IsInMultiplayer() => false; // TODO

    #region Network

    public async ValueTask OnLogin()
    {
        PlayerInstances[Uid] = this;

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
        SocialManager.LoadFromData();
        await SendPacket(new PacketPlayerStoreNotify(InventoryManager.Data.Items));
        await SendPacket(new PacketAvatarDataNotify(this));
        await SendPacket(new PacketOpenStateUpdateNotify(this));
    }

    public void OnLogoutAsync()
    {
        Chat.ChatSystem.Instance.ClearHistoryOnLogout(this);
        PlayerInstances.TryRemove(Uid, out _);
    }

    public async ValueTask SendPacket(BasePacket packet)
    {
        if (Connection?.IsOnline == true)
            await Connection.SendPacket(packet);
    }

    public async ValueTask DropMessage(string message)
        => await SendPacket(new PacketPrivateChatNotify(GameConstants.SERVER_CONSOLE_UID, Uid, message));

    public static PlayerInstance? GetPlayerInstanceByUid(uint uid)
        => PlayerInstances.TryGetValue((int)uid, out var player) ? player : null;

    #endregion

    #region Lifecycle

    public void OnPlayerBorn()
    {
        // TODO: QuestManager.OnPlayerBorn, welcome mail
    }

    public void UnfreezeUnlockedScenePoints()
    {
        // TODO: unfreeze dungeon scene locked points
    }

    public async ValueTask OnHeartBeat()
    {
        DatabaseHelper.NeedSave(Uid);
        await Task.CompletedTask;
    }

    #endregion
}
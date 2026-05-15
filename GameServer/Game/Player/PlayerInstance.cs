using System;
using NahidaImpact.Data;
using NahidaImpact.Database;
using NahidaImpact.Database.Account;
using NahidaImpact.Database.Avatar;
using NahidaImpact.Database.Player;
using NahidaImpact.GameServer.Game.Avatar;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.GameServer.Game.Friends;
using NahidaImpact.GameServer.Game.Inventory;
using NahidaImpact.GameServer.Game.Ability;
using NahidaImpact.GameServer.Game.Managers;
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
using NahidaImpact.Util.Extensions;
using NahidaImpact.Util;

namespace NahidaImpact.GameServer.Game.Player;

public class PlayerInstance
{
    public AvatarManager AvatarManager { get; private set; }
    public InventoryManager InventoryManager { get; private set; }
    public static readonly List<PlayerInstance> PlayerInstances = [];
    public PlayerData Data { get; set; }
    public EntityAvatar? EntityAvatar { get; set; }
    public List<AvatarDataInfo> Avatars => AvatarManager.AvatarData.Avatars;
    public SocialManager SocialManager { get; private set; }
    public PlayerProfile Profile { get; private set; }
    public ProgressManager ProgressManager { get; private set; }
    public AbilityManager AbilityManager { get; private set; }
    public Managers.Stamina.StaminaManager StaminaManager { get; private set; }
    public CombatInvokeHandler CombatInvokeHandler { get; private set; }
    public TeamManager TeamManager { get; private set; }
    public MapMarksManager MapMarksManager { get; private set; }
    public SotSManager? SotSManager { get; private set; }
    public Scene Scene { get; internal set; }
    public World World { get; internal set; }
    
    public uint SceneId { get; set; } = 3;
    public int Uid { get; set; }
    public uint PeerId { get; internal set; }
    public Connection? Connection { get; set; }
    public bool IsNewPlayer { get; set; }
    public bool HasSentLoginPackets { get; set; }
    public uint GuidSeed { get; set; }
    public uint EntityIdSeed { get; set; }
    public ulong GetNextGameGuid() => ((ulong)Uid << 32) + (++GuidSeed);
    public int Primogems { get; set; }
    public int Mora { get; set; }
    public int Crystals { get; set; }
    public int HomeCoin { get; set; }
    public uint EnterToken { get; set; }
    public int MainCharacterId { get; set; }
    public List<uint> ChatEmojiIdList { get; set; } = [];
    public uint WeaponEntityId = 100663300;
    
    public Position Position { get; private set; } = new Position(2747, 194, -1719); // START_POSITION from GameConstants
    public Position Rotation { get; private set; } = new Position(0, 307, 0); // Default rotation
    public Position PrevPos { get; private set; } = new Position();
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
        SocialManager = new SocialManager(this);
        ProgressManager = new ProgressManager(this);
        AbilityManager = new AbilityManager(this);
        StaminaManager = new Managers.Stamina.StaminaManager(this);
        CombatInvokeHandler = new CombatInvokeHandler(this);
        MapMarksManager = new MapMarksManager(this);

        // Initialize default player properties
        SetProperty(Prop.PlayerProp.PROP_MAX_STAMINA, 10000);
        SetProperty(Prop.PlayerProp.PROP_CUR_PERSIST_STAMINA, 10000);
        SotSManager = new SotSManager(this);
    }
    
    public void SetPosition(Position position)
    {
        Position.Set(position);
    }
    
    public void SetRotation(Position rotation)
    {
        Rotation.Set(rotation);
    }
    
    public void SetPrevPos(Position pos)
    {
        PrevPos.Set(pos);
    }
    
    public void SetPrevPosForHome(Position pos)
    {
        PrevPosForHome.Set(pos);
    }

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

        // TODO: Check HOME_SCENE_IDS like in Java
        // if (GameHome.HOME_SCENE_IDS.Contains(this.SceneId)) {
        //     this.SceneId = (uint)(this.PrevScene <= 0 ? 3 : this.PrevScene);
        //     var pos = this.PrevPosForHome;
        //     if (pos.Equals(Position.Zero)) {
        //         // TODO: Get born position from scene meta
        //         // pos = ScriptLoader.getSceneMeta(this.SceneId).config.born_pos;
        //     }
        //     this.Position.Set(pos);
        // }

        Data.LastActiveTime = Extensions.GetUnixSec();
        Profile.LastActiveTime = Data.LastActiveTime;
        
        World = new World(this);

        World.AddPlayer(this);

        // Initialize map unlocks (scene points, areas)
        ProgressManager.OnPlayerLogin();

        // Apply starting scene tags and send world scene info
        ApplyStartingSceneTags();
        await SendPacket(new PacketPlayerWorldSceneInfoListNotify(this));

        // 初始化队伍实体 (先这样写吧
        await this.TeamManager.UpdateTeamEntitiesAsync();
        
        await SendPacket(new PacketPlayerEnterSceneNotify(this));
        await SendPacket(new PacketPlayerDataNotify(this));
        HasSentLoginPackets = true;

        // Send inventory
        InventoryManager.LoadFromDatabase();
        await SendPacket(new PacketPlayerStoreNotify(InventoryManager.Data.Items));
        await SendPacket(new PacketAvatarDataNotify(this));
        await SendPacket(new PacketOpenStateUpdateNotify(this));
        
        // TODO: Multiplayer setting
        // this.setProperty(PlayerProperty.PROP_PLAYER_MP_SETTING_TYPE, this.getMpSetting().getNumber(), false);
        // this.setProperty(PlayerProperty.PROP_IS_MP_MODE_AVAILABLE, 1, false);
        
        // TODO: Execute daily reset logic if this is a new day.
        // this.doDailyReset();
        
        // TODO: Rewind active quests
        // getQuestManager().onLogin();
        
        // TODO: Handle seelie ability group
        // this.handleSeelieAbilityGroup();
        
        await Task.CompletedTask;
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
        if (Connection?.IsOnline == true) await Connection.SendPacket(packet);
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

    public Dictionary<int, MapAreaInfo> GetMapAreas() => MapAreas;

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

    public float GetPhlogistonValue()
    {
        return _phlogistonValue;
    }

    public void SetPhlogistonValue(float value)
    {
        _phlogistonValue = System.Math.Max(0, System.Math.Min(100, value));
    }


    public SatiationManager? GetSatiationManager()
    {
        // TODO: Implement satiation system
        return null;
    }

    public Managers.Stamina.StaminaManager GetStaminaManager()
    {
        return StaminaManager;
    }

    /// <summary>Player property map (MAX_STAMINA, CUR_PERSIST_STAMINA, etc.)</summary>
    private readonly Dictionary<uint, int> _playerProperties = new();

    public int GetProperty(uint propType)
    {
        return _playerProperties.TryGetValue(propType, out int value) ? value : 0;
    }

    public void SetProperty(uint propType, int value)
    {
        _playerProperties[propType] = value;
        // TODO: Send PacketPlayerPropNotify to client
    }

    public List<int> GetFlyCloakList()
    {
        // Return default flycloak
        return new List<int> { 340005 };
    }

    public List<int> GetCostumeList()
    {
        return new List<int>();
    }

    public List<int> GetTraceEffectList()
    {
        return new List<int>();
    }

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
    {
        if (!GameData.AvatarData.TryGetValue(avatarId, out var avatarExcel))
            return null;

        if (AvatarManager.HasAvatar(avatarId))
            return null;

        uint currentTimestamp = (uint)DateTimeOffset.Now.ToUnixTimeSeconds();

        var avatar = new AvatarDataInfo
        {
            AvatarId = avatarExcel.Id,
            SkillDepotId = avatarExcel.SkillDepotId,
            Guid = AvatarManager.NextGuid(),
            WeaponId = avatarExcel.InitialWeapon,
            BornTime = currentTimestamp,
            WearingFlycloakId = 340005
        };

        avatar.InitDefaultProps(avatarExcel);
        AvatarManager.AvatarData.Avatars.Add(avatar);
        AvatarManager.Save();

        // TODO: Add starting weapon (mirrors Java Player.getAvatars().addStartingWeapon(avatar))

        if (HasSentLoginPackets)
        {
            avatar.RecalcStats();
            // TODO: Send PacketAvatarAddNotify
            if (addToCurrentTeam && TeamManager != null)
                this.TeamManager.AddAvatarToCurrentTeam(avatar.Guid);
        }

        return avatar;
    }
    
    public void UnfreezeUnlockedScenePoints()
    {
        // TODO: Implement unfreeze of dungeon scenes' locked points
        // In Java: scenes.stream().filter(Scene::isDungeon).forEach(scene -> scene.unfreezeUnlockedScenePoints(player))
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
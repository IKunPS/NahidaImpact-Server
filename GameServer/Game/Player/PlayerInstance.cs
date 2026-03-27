using System;
using NahidaImpact.Database;
using NahidaImpact.Database.Account;
using NahidaImpact.Database.Avatar;
using NahidaImpact.Database.Player;
using NahidaImpact.Database.Repositories;
using NahidaImpact.GameServer.Game.Avatar;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.GameServer.Game.Friends;
using NahidaImpact.GameServer.Game.Inventory;
using NahidaImpact.GameServer.Game.Ability;
using NahidaImpact.GameServer.Game.Player.Team;
using NahidaImpact.GameServer.Game.World;
using NahidaImpact.GameServer.Server;
using NahidaImpact.GameServer.Server.Packet.Send.Avatar;
using NahidaImpact.GameServer.Server.Packet.Send.Player;
using NahidaImpact.GameServer.Server.Packet.Send.State;
using NahidaImpact.KcpSharp;
using NahidaImpact.Util.Extensions;

namespace NahidaImpact.GameServer.Game.Player;

public class PlayerInstance
{
    public AvatarManager AvatarManager { get; private set; }
    public InventoryManager InventoryManager { get; private set; }
    public static readonly List<PlayerInstance> _playerInstances = [];
    public PlayerData Data { get; set; }
    public EntityAvatar? EntityAvatar { get; set; }
    public List<AvatarDataInfo> Avatars => AvatarManager.AvatarData.Avatars;
    public SocialManager SocialManager { get; private set; }
    public PlayerProfile Profile { get; private set; }
    public ProgressManager ProgressManager { get; private set; }
    public AbilityManager AbilityManager { get; private set; }
    public TeamManager TeamManager { get; private set; }
    public Scene Scene { get; internal set; }
    public World.World World { get; internal set; }
    public uint SceneId { get; set; } = 3;
    
    public int Uid { get; set; }
    public uint PeerId { get; internal set; }
    public Connection? Connection { get; set; }
    public bool IsNewPlayer { get; set; }
    public uint GuidSeed { get; set; }
    public uint EntityIdSeed { get; set; }
    public uint EnterToken { get; set; }

    public uint WeaponEntityId = 100663300;
    
    // Position and rotation (mirroring Java implementation)
    public Position Position { get; private set; } = new Position(2747, 194, -1719); // START_POSITION from GameConstants
    public Position Rotation { get; private set; } = new Position(0, 307, 0); // Default rotation
    public Position PrevPos { get; private set; } = new Position();
    public Position PrevPosForHome { get; private set; } = Position.Zero;
    public int PrevScene { get; set; }
    
    public PlayerInstance(PlayerData data)
    {
        Data = data;
        Uid = data.Uid;
        Profile = new PlayerProfile(data.Name ?? "Traveler");
        
        ITeamRepository teamRepository = new TeamRepository(DatabaseHelper.sqlSugarScope!);
        TeamManager = new TeamManager(this, teamRepository);
        AvatarManager = new AvatarManager(this);
        InventoryManager = new InventoryManager(this);
        SocialManager = new SocialManager(this);
        ProgressManager = new ProgressManager(this);
        AbilityManager = new AbilityManager(this);
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
        _playerInstances.Add(this);

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
        
        World = new World.World(this);
        
        await AvatarManager.InitializeDefaultAvatar();
        
        World.AddPlayer(this);
        
        // 初始化队伍实体 (先这样写吧
        await TeamManager.UpdateTeamEntitiesAsync();
        
        await SendPacket(new PacketPlayerEnterSceneNotify(this));
        await SendPacket(new PacketPlayerDataNotify(this));
        await SendPacket(new PacketAvatarDataNotify(this, Avatars!));
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
        => _playerInstances.FirstOrDefault(player => player.Uid == uid);
    
    public void OnLogoutAsync()
    {
        _playerInstances.Remove(this);
    }
    public async ValueTask SendPacket(BasePacket packet)
    {
        if (Connection?.IsOnline == true) await Connection.SendPacket(packet);
    }

    #endregion

    #region Helpers
    public bool IsInMultiplayer()
    {
        // TODO: Implement multiplayer check
        return false;
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
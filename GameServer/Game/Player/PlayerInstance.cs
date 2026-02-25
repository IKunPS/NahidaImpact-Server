using NahidaImpact.Data;
using NahidaImpact.Database;
using NahidaImpact.Database.Account;
using NahidaImpact.Database.Avatar;
using NahidaImpact.Database.Player;
using NahidaImpact.GameServer.Game.Avatar;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.GameServer.Game.Friends;
using NahidaImpact.GameServer.Game.Inventory;
using NahidaImpact.GameServer.Game.Scene;
using NahidaImpact.GameServer.Server;
using NahidaImpact.GameServer.Server.Packet.Send.Avatar;
using NahidaImpact.GameServer.Server.Packet.Send.Player;
using NahidaImpact.GameServer.Server.Packet.Send.State;
using NahidaImpact.KcpSharp;
using NahidaImpact.Util.Extensions;

namespace NahidaImpact.GameServer.Game.Player;

public class PlayerInstance(PlayerData data)
{
    public AvatarManager? AvatarManager { get; private set; }
    public InventoryManager? InventoryManager { get; private set; }
    public static readonly List<PlayerInstance> _playerInstances = [];
    public PlayerData Data { get; set; } = data;
    public SceneManager? SceneManager { get; private set; }
    public WorldChatManager? WorldChatManager { get; private set; }
    public EntityManager? EntityManager { get; private set; }
    public EntityAvatar? EntityAvatar { get; set; }
    public List<AvatarDataInfo>? Avatars { get; set; }
    public SocialManager? SocialManager { get; private set; }
    public PlayerProfile Profile { get; private set; } = null!;
    
    public int Uid { get; set; }
    public uint SceneId { get; set; } = 3;
    public Connection? Connection { get; set; }
    public bool Initialized { get; set; }
    public bool IsNewPlayer { get; set; }
    public uint GuidSeed { get; set; }
    public uint EntityIdSeed { get; set; }

    public uint WeaponEntityId = 100663300;

    #region Initializers
    public PlayerInstance(int uid) : this(new PlayerData { Uid = uid })
    {
        // new player
        IsNewPlayer = true;
        Data.Name = AccountData.GetAccountByUid(uid)?.Username;
        Profile = new PlayerProfile(Data.Name ?? "Traveler");

        DatabaseHelper.CreateInstance(Data);
    }
    private async ValueTask InitialPlayerManager()
    {
        Uid = Data.Uid;
        Profile ??= new PlayerProfile(Data.Name ?? "Traveler");

        AvatarManager = new AvatarManager(this);
        InventoryManager = new InventoryManager(this);
        WorldChatManager = new WorldChatManager(this);
        EntityManager = new EntityManager(this);
        SceneManager = new SceneManager(this);
        SocialManager = new SocialManager(this);
        Avatars = AvatarManager.AvatarData.Avatars;

        Data.LastActiveTime = Extensions.GetUnixSec();
        Profile.LastActiveTime = Data.LastActiveTime;

        await AvatarManager!.AddAvatar(10000007); // TODO 在其他地方
        await SceneManager.EnterSceneAsync(SceneId);

        Initialized = true;
    }

    public T InitializeDatabase<T>() where T : BaseDatabaseDataHelper, new()
    {
        var instance = DatabaseHelper.GetInstanceOrCreateNew<T>(Uid);
        return instance!;
    }

    #endregion

    #region Network
    public async ValueTask OnGetToken()
    {
        if (!Initialized) await InitialPlayerManager();
    }

    public async ValueTask OnLogin()
    {
        _playerInstances.Add(this);

        await SendPacket(new PacketPlayerEnterSceneNotify(this));
        await SendPacket(new PacketPlayerDataNotify(this));
        await SendPacket(new PacketAvatarDataNotify(this, Avatars!));
        await SendPacket(new PacketOpenStateUpdateNotify());
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

    #region Actions
    public async ValueTask OnHeartBeat()
    {
        DatabaseHelper.ToSaveUidList.SafeAdd(Uid);
        await Task.CompletedTask;
    }

    #endregion
}
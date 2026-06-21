namespace NahidaImpact.Internationalization.Message;

#region Root

/// <summary>
///     Simplified Chinese (简体中文)
/// </summary>
public class LanguageCHS
{
    public GameTextCHS Game { get; } = new();
    public ServerTextCHS Server { get; } = new();
    public WordTextCHS Word { get; } = new();
}

#endregion

#region Layer 1

/// <summary>
///     path: Game
/// </summary>
public class GameTextCHS
{
    public CommandTextCHS Command { get; } = new();
    public SceneInfoTextCHS SceneInfo { get; } = new();
    public WorldInfoTextCHS WorldInfo { get; } = new();
}

/// <summary>
///     path: Server
/// </summary>
public class ServerTextCHS
{
    public WebTextCHS Web { get; } = new();
    public ServerInfoTextCHS ServerInfo { get; } = new();
    public ConnectionInfoTextCHS ConnectionInfo { get; } = new();
}

/// <summary>
///     path: Word
/// </summary>
public class WordTextCHS
{
    public string Config => "配置文件";
    public string Language => "语言";
    public string GameData => "游戏数据";
    public string CustomData => "自定义数据";
    public string Database => "数据库";
    public string Command => "命令";
    public string Dispatch => "全局分发";
    public string Game => "游戏";
    public string Handbook => "手册";
    public string NotFound => "未找到";
    public string Error => "错误";
    public string DatabaseAccount => "数据库账号";
    public string Resource => "资源";
}

#endregion

#region Layer 2

/// <summary>
///     path: Server.Web
/// </summary>
public class WebTextCHS
{
    public string Maintain => "服务器正在维护，请稍后再试。";
    public string AccountNotFound => "账号未找到";
    public string PasswordIncorrect => "密码错误";
    public string LoginFailed => "账号登录失败";
    public string DecryptionFailed => "账号名解密失败";
    public string CreateAccountFailed => "创建账号失败";
    public string TokenError => "账号令牌错误";
    public string Relogin => "为了账号安全，请重新登录";
    public string OK => "OK";
    public string CacheError => "账号缓存错误";
    public string ConnectionFailed => "连接失败！";
}

/// <summary>
///     path: Server.ServerInfo
/// </summary>
public class ServerInfoTextCHS
{
    public string Shutdown => "关闭中…";
    public string CancelKeyPressed => "已按下取消键 (Ctrl + C)，服务器即将关闭…";
    public string StartingServer => "正在启动 NahidaImpact";
    public string CurrentVersion => "当前服务端支持的版本: {0}";
    public string LoadingItem => "正在加载 {0}…";
    public string GeneratingItem => "正在生成 {0}…";
    public string RegisterItem => "注册了 {0} 个 {1}。";
    public string FailedToLoadItem => "加载 {0} 失败。";
    public string FailedToInitializeItem => "初始化 {0} 失败。";
    public string FailedToReadItem => "读取 {0} 失败，文件{1}";
    public string GeneratedItem => "已生成 {0}。";

    public string LoadedItem => "已加载 {0}。";
    public string LoadedItems => "已加载 {0} 个 {1}。";
    public string ServerRunning => "{0} 服务器正在监听 {1}";
    public string ServerStarted => "启动完成！用时 {0}s，输入 'help' 来获取命令帮助";
    public string ConfigMissing => "{0} 缺失，请检查你的资源文件夹: {1}，{2} 可能不能使用。";
    public string SaveDatabase => "已保存数据库，用时 {0}s";
    public string WaitForAllDone => "现在还不可以进入游戏，请等待所有项目加载完成后再试";
    public string UnhandledException => "发生未经处理的异常: {0}";
    public string DirNotFound => "{0} 目录未找到: {1}";
    public string FailedToLoadData => "加载 {0} 数据失败。";
    public string FailedToLoadFile => "加载 {0} 失败: {1}";

    public string LoadedScenePoints => "已加载 {0} 个场景传送点，覆盖 {1} 个场景";
    public string FileNotFound => "{0} 文件未找到: {1}";
}

/// <summary>
///     path: Game.Command
/// </summary>
public class CommandTextCHS
{
    public NoticeTextCHS Notice { get; } = new();
    public HelpTextCHS Help { get; } = new();
    public GiveTextCHS Give { get; } = new();
    public GiveAllTextCHS GiveAll { get; } = new();
    public TeleportTextCHS Teleport { get; } = new();
    public AccountTextCHS Account { get; } = new();
    public KickTextCHS Kick { get; } = new();
    public SpawnTextCHS Spawn { get; } = new();
    public StopTextCHS Stop { get; } = new();
}

#endregion

#region Layer 3 - Command Texts

/// <summary>
///     path: Game.Command.Notice
/// </summary>
public class NoticeTextCHS
{
    public string PlayerNotFound => "未找到玩家！";
    public string InvalidArguments => "无效的参数！";
    public string NoPermission => "你没有权限这么做！";
    public string CommandNotFound => "未找到命令！输入 '/help' 来获取帮助";
    public string InternalError => "在处理命令时发生了内部错误: {0}！";
}

/// <summary>
///     path: Game.Command.Help
/// </summary>
public class HelpTextCHS
{
    public string Desc => "显示帮助信息";
    public string Usage => "用法: /help\n用法: /help [命令]";
    public string Commands => "命令: ";
    public string CommandPermission => "所需权限: ";
    public string CommandAlias => "命令别名: ";
}

/// <summary>
///     path: Game.Command.Give
/// </summary>
public class GiveTextCHS
{
    public string Desc => "给予玩家物品";
    public string Usage => "用法: /give <itemId> [amount] [level]";
    public string InvalidItemId => "无效的物品ID。";
    public string Success => "已给予 {0}x 物品 {1} (等级 {2}) 给玩家 {3}。";
    public string Failed => "无法给予物品 {0}，可能尚不支持该物品类型。";
}

/// <summary>
///     path: Game.Command.GiveAll
/// </summary>
public class GiveAllTextCHS
{
    public string Desc => "给予玩家全部指定类型的物品";
    public string Usage => "用法: /give <all|weapons|relics|mats|furniture|avatars> [lv<等级>] [x<数量>] [r<精炼>]";
    public string AvatarsGiven => "已给予 {0} 个角色 (UID {1})。";
    public string WeaponsGiven => "已给予 {0} 把武器 (UID {3})。";
    public string RelicsGiven => "已给予 {0} 件圣遗物 (UID {3})。";
    public string MaterialsGiven => "已给予 {0} 种材料 x{1} (UID {3})。";
    public string FurnitureGiven => "已给予 {0} 件摆设 x{1} (UID {3})。";
    public string ItemsGiven => "已给予 {0} 个物品 (UID {3})。";
    public string AllGiven => "全部物品已给予 UID {0}。";
}

/// <summary>
///     path: Game.Command.Teleport
/// </summary>
public class TeleportTextCHS
{
    public string Desc => "传送玩家到指定坐标";
    public string Usage => "用法: /tp <x> <y> <z> [sceneId]";
    public string InvalidCoords => "无效的坐标。";
    public string Success => "已将 {0} 传送到 ({1:F0}, {2:F0}, {3:F0}) 场景 {4}。";
    public string Failed => "传送失败，场景 {0} 可能不存在。";
}

/// <summary>
///     path: Game.Command.Account
/// </summary>
public class AccountTextCHS
{
    public string Desc => "管理数据库账号";
    public string Usage => "用法: /account <create|delete> [username] [uid]";
    public string InvalidUid => "UID无效！";
    public string CreateSuccess => "账号 '{0}' 创建成功，UID: {1}。";
    public string DeleteSuccess => "账号 UID {0} 已删除。";
}

/// <summary>
///     path: Game.Command.Kick
/// </summary>
public class KickTextCHS
{
    public string Desc => "踢出玩家";
    public string Usage => "用法: /kick @<uid>";
    public string PlayerKicked => "玩家 {0} 已被踢出！";
    public string KickMessage => "你已被服务器踢出。";
}

/// <summary>
///     path: Game.Command.Spawn
/// </summary>
public class SpawnTextCHS
{
    public string Desc => "生成实体";
    public string Usage => "用法: /spawn <id> [x<数量>] [lv<等级>] [state<状态>] [ai<aiId>] [<x> <y> <z>]";
    public string InvalidEntityId => "无效的实体ID。";
    public string NoScene => "玩家不在场景中。";
    public string Success => "已生成 {0} 个 {1} (ID: {2}, Lv: {3}) (UID {4})。";
}

/// <summary>
///     path: Game.Command.Stop
/// </summary>
public class StopTextCHS
{
    public string Desc => "关闭服务器";
    public string Usage => "用法: /stop";
    public string ShuttingDown => "服务器正在关闭…";
}

#endregion

#region Layer 3 - Connection / Scene / World log messages

public class ConnectionInfoTextCHS
{
    public string NewConnection => "新连接来自 {0}。";
    public string ConnectionClosed => "连接已关闭";
    public string PacketTooLarge => "数据包过大";
    public string FailedToReceive => "接收数据包失败";
    public string ParseError => "数据包解析错误";
    public string BadDataPackage => "收到损坏数据包: 包头为 0x{0}，期望 0x4567";
    public string InvalidFooter => "收到无效数据包尾: 包尾为 0x{0}，期望 0x89AB";
    public string DummySend => "[Dummy] 发送 Dummy {0}";
    public string NoHandlerFound => "未找到数据包 {0} 的处理程序 (0x{1})";
    public string RegionCacheError => "初始化区域缓存时出错！";
}

public class SceneInfoTextCHS
{
    public string NoTeamManagerSpawn => "玩家 {0} 没有 TeamManager，无法生成角色";
    public string NoAvatarEntity => "玩家 {0} 没有当前角色实体可生成";
    public string NoTeamManagerSetup => "玩家 {0} 没有 TeamManager，无法设置角色";
    public string NullEntity => "尝试向场景 {0} 添加空实体";
    public string NoAvatarData => "未找到角色数据！请检查你的 ExcelBinOutput 文件夹。";
}

public class WorldInfoTextCHS
{
    public string NotifyPlayerInfo => "通知玩家 {0} 有关新玩家 {1} 的信息";
}

#endregion
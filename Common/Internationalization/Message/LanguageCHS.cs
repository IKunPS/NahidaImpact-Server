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
}

/// <summary>
///     path: Server
/// </summary>
public class ServerTextCHS
{
    public WebTextCHS Web { get; } = new();
    public ServerInfoTextCHS ServerInfo { get; } = new();
}

/// <summary>
///     path: Word
/// </summary>
public class WordTextCHS
{
    public string Config => "配置文件";
    public string Language => "语言";
    public string Log => "日志";
    public string GameData => "游戏数据";
    public string Cache => "资源缓存";
    public string CustomData => "自定义数据";
    public string Database => "数据库";
    public string Command => "命令";
    public string SdkServer => "Web服务器";
    public string Handler => "包处理器";
    public string Dispatch => "全局分发";
    public string Game => "游戏";
    public string Handbook => "手册";
    public string NotFound => "未找到";
    public string Error => "错误";
    public string DatabaseAccount => "数据库账号";
    public string Tutorial => "教程";
}

#endregion

#region Layer 2

/// <summary>
///     path: Server.Web
/// </summary>
public class WebTextCHS
{
    public string Maintain => "服务器正在维护，请稍后再试。";
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
    public string InvalidVersion => "当前为不受支持的游戏版本 {0}\n请更新游戏到 {1}";
    public string LoadingItem => "正在加载 {0}…";
    public string GeneratingItem => "正在生成 {0}…";
    public string WaitingItem => "正在等待进程 {0} 完成…";
    public string RegisterItem => "注册了 {0} 个 {1}。";
    public string FailedToLoadItem => "加载 {0} 失败。";
    public string NewClientSecretKey => "客户端密钥不存在，正在生成新的客户端密钥。";
    public string FailedToInitializeItem => "初始化 {0} 失败。";
    public string FailedToReadItem => "读取 {0} 失败，文件{1}";
    public string GeneratedItem => "已生成 {0}。";
    public string LoadedItem => "已加载 {0}。";
    public string LoadedItems => "已加载 {0} 个 {1}。";
    public string ServerRunning => "{0} 服务器正在监听 {1}";
    public string ServerStarted => "启动完成！用时 {0}s，输入 'help' 来获取命令帮助";
    public string MissionEnabled => "任务系统已启用，此功能仍在开发中。";
    public string KeyStoreError => "SSL证书不存在，已关闭SSL功能。";
    public string CacheLoadSkip => "已跳过缓存加载。";
    public string ConfigMissing => "{0} 缺失，请检查你的资源文件夹: {1}，{2} 可能不能使用。";
    public string UnloadedItems => "卸载了所有 {0}。";
    public string SaveDatabase => "已保存数据库，用时 {0}s";
    public string WaitForAllDone => "现在还不可以进入游戏，请等待所有项目加载完成后再试";
    public string UnhandledException => "发生未经处理的异常: {0}";
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
    public HealTextCHS Heal { get; } = new();
    public ClearTextCHS Clear { get; } = new();
    public AccountTextCHS Account { get; } = new();
    public PermissionTextCHS Permission { get; } = new();
    public KickTextCHS Kick { get; } = new();
    public ListTextCHS List { get; } = new();
    public SetPropTextCHS SetProp { get; } = new();
    public UnlockAllTextCHS UnlockAll { get; } = new();
    public SpawnTextCHS Spawn { get; } = new();
    public KillAllTextCHS KillAll { get; } = new();
    public MapAreaTextCHS MapArea { get; } = new();
    public WeatherTextCHS Weather { get; } = new();
    public PositionTextCHS Position { get; } = new();
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
    public string PlayerNotInit => "玩家未初始化。";
    public string InvalidArguments => "无效的参数！";
    public string NoPermission => "你没有权限这么做！";
    public string CommandNotFound => "未找到命令！输入 '/help' 来获取帮助";
    public string TargetOffline => "目标 {0}({1}) 离线了！";
    public string TargetFound => "找到目标 {0}({1})。";
    public string TargetNotFound => "未找到目标 {0}！";
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
    public string Usage => "用法: /giveall <avatars|weapons|relics|materials|essentials|all> [数量]";
    public string GiveAllItems => "已给予所有 {0} x{1}。";
    public string AvatarsGiven => "已给予玩家 {1} {0} 个角色。";
    public string MaterialsGiven => "已给予玩家 {2} {0} 种材料 x{1}。";
    public string WeaponsGiven => "已给予玩家 {1} {0} 把武器。";
    public string RelicsGiven => "已给予玩家 {1} {0} 件圣遗物。";
    public string EssentialsGiven => "已给予玩家 {2} {0} 种必备物品 x{1}。";
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
///     path: Game.Command.Heal
/// </summary>
public class HealTextCHS
{
    public string Desc => "治愈当前队伍所有角色";
    public string Usage => "用法: /heal";
    public string NoTeam => "玩家没有队伍。";
    public string Success => "已治愈玩家 {0} 的所有角色。";
}

/// <summary>
///     path: Game.Command.Clear
/// </summary>
public class ClearTextCHS
{
    public string Desc => "清除玩家背包";
    public string Usage => "用法: /clear <weapons|materials|all>";
    public string ClearedAll => "已清除玩家 {0} 的所有物品。";
    public string ClearedMaterials => "已清除玩家 {0} 的所有材料。";
    public string ClearedWeapons => "已清除玩家 {0} 的所有武器。";
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
///     path: Game.Command.Permission
/// </summary>
public class PermissionTextCHS
{
    public string Desc => "管理玩家权限";
    public string Usage => "用法: /perm <add|remove|clear|list> [permission]";
    public string AccountNotFound => "未找到账号。";
    public string InvalidPerm => "无效的权限。有效值: {0}";
    public string Added => "已为 UID {1} 添加权限 '{0}'。";
    public string Removed => "已从 UID {1} 移除权限 '{0}'。";
    public string Cleared => "已清除 UID {0} 的所有权限。";
    public string List => "UID {0} 的权限: {1}";
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
///     path: Game.Command.List
/// </summary>
public class ListTextCHS
{
    public string Desc => "列出在线玩家";
    public string Usage => "用法: /list";
    public string Header => "在线玩家 ({0}):";
    public string Entry => "  UID: {0} | 名称: {1} | 场景: {2} | 位置: ({3:F0}, {4:F0}, {5:F0})";
}

/// <summary>
///     path: Game.Command.SetProp
/// </summary>
public class SetPropTextCHS
{
    public string Desc => "设定玩家属性";
    public string Usage => "用法: /setprop <worldLevel|exp|name|signature> <value>";
    public string WorldLevel => "已将玩家 {1} 的世界等级设为 {0}。";
    public string Exp => "已将玩家 {1} 的经验设为 {0}。";
    public string Name => "已将玩家 {1} 的名称设为 '{0}'。";
    public string Signature => "已将玩家 {1} 的签名设为 '{0}'。";
    public string Unknown => "未知属性: {0}";
}

/// <summary>
///     path: Game.Command.UnlockAll
/// </summary>
public class UnlockAllTextCHS
{
    public string Desc => "解锁所有场景传送点";
    public string Usage => "用法: /unlockall";
    public string Success => "已为玩家 {1} 解锁 {0} 个场景传送点 (场景 {2})。";
}

/// <summary>
///     path: Game.Command.Spawn
/// </summary>
public class SpawnTextCHS
{
    public string Desc => "生成怪物或物件";
    public string Usage => "用法: /spawn <monster|gadget> <id> [数量] [等级]";
    public string InvalidEntityId => "无效的实体ID。";
    public string NoScene => "玩家不在场景中。";
    public string Success => "已在玩家 {4} 附近生成 {0} 个 {1} (ID: {2}, Lv: {3})。";
}

/// <summary>
///     path: Game.Command.KillAll
/// </summary>
public class KillAllTextCHS
{
    public string Desc => "击杀场景内所有怪物";
    public string Usage => "用法: /killall";
    public string Success => "已击杀玩家 {0} 当前场景内所有怪物。";
}

/// <summary>
/// <summary>
///     path: Game.Command.MapArea
/// </summary>
public class MapAreaTextCHS
{
    public string Desc => "管理地图区域";
    public string Usage => "用法: /maparea <give|remove|isopen> ...";
    public string Unknown => "未知子命令: {0}";
    public string GiveUsage => "用法: /maparea give <id|all> [isOpen]";
    public string GiveAll => "已给予玩家 {0} 所有地图区域 (isOpen={1})";
    public string GiveOne => "已给予玩家 {1} 地图区域 {0} (isOpen={2})";
    public string RemoveUsage => "用法: /maparea remove <id>";
    public string RemoveSuccess => "已从玩家 {1} 移除地图区域 {0}";
    public string SetOpenUsage => "用法: /maparea isopen <id> <true|false>";
    public string SetOpenSuccess => "已设置地图区域 {0} isOpen={1} 给玩家 {2}";
    public string InvalidId => "无效的地图区域ID: {0}";
    public string NotFound => "玩家 {1} 未找到地图区域 {0}";
    public string InvalidArgs => "无效参数: id={0}, isOpen={1}";
}

/// <summary>
///     path: Game.Command.Weather
/// </summary>
public class WeatherTextCHS
{
    public string Desc => "更改场景天气";
    public string Usage => "用法: /weather <0-5>";
    public string Invalid => "无效的天气类型。范围: 0-5";
    public string Success => "已将玩家 {1} 的天气设为类型 {0}。";
}

/// <summary>
///     path: Game.Command.Position
/// </summary>
public class PositionTextCHS
{
    public string Desc => "显示当前位置";
    public string Usage => "用法: /pos";
    public string PlayerInfo => "玩家: {0} | 场景: {1}";
    public string PositionInfo => "位置: X={0:F1} Y={1:F1} Z={2:F1}";
    public string RotationInfo => "旋转: X={0:F1} Y={1:F1} Z={2:F1}";
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

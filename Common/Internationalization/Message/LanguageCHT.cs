namespace NahidaImpact.Internationalization.Message;

#region Root

/// <summary>
///     Traditional Chinese (繁體中文)
/// </summary>
public class LanguageCHT
{
    public GameTextCHT Game { get; } = new();
    public ServerTextCHT Server { get; } = new();
    public WordTextCHT Word { get; } = new();
}

#endregion

#region Layer 1

/// <summary>
///     path: Game
/// </summary>
public class GameTextCHT
{
    public CommandTextCHT Command { get; } = new();
}

/// <summary>
///     path: Server
/// </summary>
public class ServerTextCHT
{
    public WebTextCHT Web { get; } = new();
    public ServerInfoTextCHT ServerInfo { get; } = new();
}

/// <summary>
///     path: Word
/// </summary>
public class WordTextCHT
{
    public string Config => "配置文件";
    public string Language => "語言";
    public string Log => "日誌";
    public string GameData => "遊戲數據";
    public string Cache => "資源緩存";
    public string CustomData => "自定義數據";
    public string Database => "數據庫";
    public string Command => "命令";
    public string SdkServer => "Web服務器";
    public string Handler => "包處理器";
    public string Dispatch => "全局分發";
    public string Game => "遊戲";
    public string Handbook => "手冊";
    public string NotFound => "未找到";
    public string Error => "錯誤";
    public string DatabaseAccount => "數據庫賬號";
    public string Tutorial => "教程";
}

#endregion

#region Layer 2

/// <summary>
///     path: Server.Web
/// </summary>
public class WebTextCHT
{
    public string Maintain => "服務器正在維修，請稍後嘗試。";
}

/// <summary>
///     path: Server.ServerInfo
/// </summary>
public class ServerInfoTextCHT
{
    public string Shutdown => "關閉中…";
    public string CancelKeyPressed => "已按下取消鍵 (Ctrl + C)，服務器即將關閉…";
    public string StartingServer => "正在啟動 NahidaImpact";
    public string CurrentVersion => "當前服務端支援的版本: {0}";
    public string InvalidVersion => "目前為不受支援的遊戲版本 {0}\n請更新遊戲到 {1}";
    public string LoadingItem => "正在加載 {0}…";
    public string GeneratingItem => "正在生成 {0}…";
    public string WaitingItem => "正在等待進程 {0} 完成…";
    public string RegisterItem => "註冊了 {0} 個 {1}。";
    public string FailedToLoadItem => "加載 {0} 失敗。";
    public string NewClientSecretKey => "客戶端密鑰不存在，正在生成新的客戶端密鑰。";
    public string FailedToInitializeItem => "初始化 {0} 失敗。";
    public string FailedToReadItem => "讀取 {0} 失敗，文件{1}";
    public string GeneratedItem => "已生成 {0}。";
    public string LoadedItem => "已加載 {0}。";
    public string LoadedItems => "已加載 {0} 個 {1}。";
    public string ServerRunning => "{0} 服務器正在監聽 {1}";
    public string ServerStarted => "啟動完成！用時 {0}s，輸入 『help』 來獲取命令幫助";
    public string MissionEnabled => "任務系統已啟用，此功能仍在開發中。";
    public string KeyStoreError => "SSL證書不存在，已關閉SSL功能。";
    public string CacheLoadSkip => "已跳過緩存加載。";
    public string ConfigMissing => "{0} 缺失，請檢查你的資源文件夾: {1}，{2} 可能不能使用。";
    public string UnloadedItems => "卸載了所有 {0}。";
    public string SaveDatabase => "已保存數據庫，用時 {0}s";
    public string WaitForAllDone => "現在還不可以進入遊戲，請等待所有項目加載完成後再試";
    public string UnhandledException => "發生未經處理的異常: {0}";
}

/// <summary>
///     path: Game.Command
/// </summary>
public class CommandTextCHT
{
    public NoticeTextCHT Notice { get; } = new();
    public HelpTextCHT Help { get; } = new();
    public GiveTextCHT Give { get; } = new();
    public GiveAllTextCHT GiveAll { get; } = new();
    public TeleportTextCHT Teleport { get; } = new();
    public HealTextCHT Heal { get; } = new();
    public ClearTextCHT Clear { get; } = new();
    public AccountTextCHT Account { get; } = new();
    public PermissionTextCHT Permission { get; } = new();
    public KickTextCHT Kick { get; } = new();
    public ListTextCHT List { get; } = new();
    public SetPropTextCHT SetProp { get; } = new();
    public UnlockAllTextCHT UnlockAll { get; } = new();
    public SpawnTextCHT Spawn { get; } = new();
    public KillAllTextCHT KillAll { get; } = new();
    public MapAreaTextCHT MapArea { get; } = new();
    public WeatherTextCHT Weather { get; } = new();
    public PositionTextCHT Position { get; } = new();
    public StopTextCHT Stop { get; } = new();
}

#endregion

#region Layer 3 - Command Texts

/// <summary>
///     path: Game.Command.Notice
/// </summary>
public class NoticeTextCHT
{
    public string PlayerNotFound => "未找到玩家！";
    public string PlayerNotInit => "玩家未初始化。";
    public string InvalidArguments => "無效的參數！";
    public string NoPermission => "你沒有權限這麼做！";
    public string CommandNotFound => "未找到命令！輸入 '/help' 來獲取幫助";
    public string TargetOffline => "目標 {0}({1}) 離線了！";
    public string TargetFound => "找到目標 {0}({1})。";
    public string TargetNotFound => "未找到目標 {0}！";
    public string InternalError => "在處理命令時發生了內部錯誤: {0}！";
}

/// <summary>
///     path: Game.Command.Help
/// </summary>
public class HelpTextCHT
{
    public string Desc => "顯示幫助信息";
    public string Usage => "用法: /help\n用法: /help [命令]";
    public string Commands => "命令: ";
    public string CommandPermission => "所需權限: ";
    public string CommandAlias => "命令別名: ";
}

/// <summary>
///     path: Game.Command.Give
/// </summary>
public class GiveTextCHT
{
    public string Desc => "給予玩家物品";
    public string Usage => "用法: /give <itemId> [amount] [level]";
    public string InvalidItemId => "無效的物品ID。";
    public string Success => "已給予 {0}x 物品 {1} (等級 {2}) 給玩家 {3}。";
    public string Failed => "無法給予物品 {0}，可能尚不支持該物品類型。";
}

/// <summary>
///     path: Game.Command.GiveAll
/// </summary>
public class GiveAllTextCHT
{
    public string Desc => "給予玩家全部指定類型的物品";
    public string Usage => "用法: /giveall <avatars|weapons|relics|materials|essentials|all> [數量]";
    public string GiveAllItems => "已給予所有 {0} x{1}。";
    public string AvatarsGiven => "已給予玩家 {1} {0} 個角色。";
    public string MaterialsGiven => "已給予玩家 {2} {0} 種材料 x{1}。";
    public string WeaponsGiven => "已給予玩家 {1} {0} 把武器。";
    public string RelicsGiven => "已給予玩家 {1} {0} 件聖遺物。";
    public string EssentialsGiven => "已給予玩家 {2} {0} 種必備物品 x{1}。";
}

/// <summary>
///     path: Game.Command.Teleport
/// </summary>
public class TeleportTextCHT
{
    public string Desc => "傳送玩家到指定坐標";
    public string Usage => "用法: /tp <x> <y> <z> [sceneId]";
    public string InvalidCoords => "無效的坐標。";
    public string Success => "已將 {0} 傳送到 ({1:F0}, {2:F0}, {3:F0}) 場景 {4}。";
    public string Failed => "傳送失敗，場景 {0} 可能不存在。";
}

/// <summary>
///     path: Game.Command.Heal
/// </summary>
public class HealTextCHT
{
    public string Desc => "治愈當前隊伍所有角色";
    public string Usage => "用法: /heal";
    public string NoTeam => "玩家沒有隊伍。";
    public string Success => "已治愈玩家 {0} 的所有角色。";
}

/// <summary>
///     path: Game.Command.Clear
/// </summary>
public class ClearTextCHT
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
public class AccountTextCHT
{
    public string Desc => "管理資料庫帳號";
    public string Usage => "用法: /account <create|delete> [username] [uid]";
    public string InvalidUid => "UID無效！";
    public string CreateSuccess => "賬號 '{0}' 創建成功，UID: {1}。";
    public string DeleteSuccess => "帳號 UID {0} 已刪除。";
}

/// <summary>
///     path: Game.Command.Permission
/// </summary>
public class PermissionTextCHT
{
    public string Desc => "管理玩家權限";
    public string Usage => "用法: /perm <add|remove|clear|list> [permission]";
    public string AccountNotFound => "未找到帳號。";
    public string InvalidPerm => "無效的權限。有效值: {0}";
    public string Added => "已為 UID {1} 添加權限 '{0}'。";
    public string Removed => "已從 UID {1} 移除權限 '{0}'。";
    public string Cleared => "已清除 UID {0} 的所有權限。";
    public string List => "UID {0} 的權限: {1}";
}

/// <summary>
///     path: Game.Command.Kick
/// </summary>
public class KickTextCHT
{
    public string Desc => "踢出玩家";
    public string Usage => "用法: /kick @<uid>";
    public string PlayerKicked => "玩家 {0} 已被踢出！";
    public string KickMessage => "你已被伺服器踢出。";
}

/// <summary>
///     path: Game.Command.List
/// </summary>
public class ListTextCHT
{
    public string Desc => "列出在線玩家";
    public string Usage => "用法: /list";
    public string Header => "在線玩家 ({0}):";
    public string Entry => "  UID: {0} | 名稱: {1} | 場景: {2} | 位置: ({3:F0}, {4:F0}, {5:F0})";
}

/// <summary>
///     path: Game.Command.SetProp
/// </summary>
public class SetPropTextCHT
{
    public string Desc => "設定玩家屬性";
    public string Usage => "用法: /setprop <worldLevel|exp|name|signature> <value>";
    public string WorldLevel => "已將玩家 {1} 的世界等級設為 {0}。";
    public string Exp => "已將玩家 {1} 的經驗設為 {0}。";
    public string Name => "已將玩家 {1} 的名稱設為 '{0}'。";
    public string Signature => "已將玩家 {1} 的簽名設為 '{0}'。";
    public string Unknown => "未知屬性: {0}";
}

/// <summary>
///     path: Game.Command.UnlockAll
/// </summary>
public class UnlockAllTextCHT
{
    public string Desc => "解鎖所有場景傳送點";
    public string Usage => "用法: /unlockall";
    public string Success => "已為玩家 {1} 解鎖 {0} 個場景傳送點 (場景 {2})。";
}

/// <summary>
///     path: Game.Command.Spawn
/// </summary>
public class SpawnTextCHT
{
    public string Desc => "生成怪物或物件";
    public string Usage => "用法: /spawn <monster|gadget> <id> [數量] [等級]";
    public string InvalidEntityId => "無效的實體ID。";
    public string NoScene => "玩家不在場景中。";
    public string Success => "已在玩家 {4} 附近生成 {0} 個 {1} (ID: {2}, Lv: {3})。";
}

/// <summary>
///     path: Game.Command.KillAll
/// </summary>
public class KillAllTextCHT
{
    public string Desc => "擊殺場景內所有怪物";
    public string Usage => "用法: /killall";
    public string Success => "已擊殺玩家 {0} 當前場景內所有怪物。";
}

/// <summary>
/// <summary>
///     path: Game.Command.MapArea
/// </summary>
public class MapAreaTextCHT
{
    public string Desc => "管理地圖區域";
    public string Usage => "用法: /maparea <give|remove|isopen> ...";
    public string Unknown => "未知子命令: {0}";
    public string GiveUsage => "用法: /maparea give <id|all> [isOpen]";
    public string GiveAll => "已給予玩家 {0} 所有地圖區域 (isOpen={1})";
    public string GiveOne => "已給予玩家 {1} 地圖區域 {0} (isOpen={2})";
    public string RemoveUsage => "用法: /maparea remove <id>";
    public string RemoveSuccess => "已從玩家 {1} 移除地圖區域 {0}";
    public string SetOpenUsage => "用法: /maparea isopen <id> <true|false>";
    public string SetOpenSuccess => "已設定地圖區域 {0} isOpen={1} 給玩家 {2}";
    public string InvalidId => "無效的地圖區域ID: {0}";
    public string NotFound => "玩家 {1} 未找到地圖區域 {0}";
    public string InvalidArgs => "無效參數: id={0}, isOpen={1}";
}

/// <summary>
///     path: Game.Command.Weather
/// </summary>
public class WeatherTextCHT
{
    public string Desc => "更改場景天氣";
    public string Usage => "用法: /weather <0-5>";
    public string Invalid => "無效的天氣類型。範圍: 0-5";
    public string Success => "已將玩家 {1} 的天氣設為類型 {0}。";
}

/// <summary>
///     path: Game.Command.Position
/// </summary>
public class PositionTextCHT
{
    public string Desc => "顯示當前位置";
    public string Usage => "用法: /pos";
    public string PlayerInfo => "玩家: {0} | 場景: {1}";
    public string PositionInfo => "位置: X={0:F1} Y={1:F1} Z={2:F1}";
    public string RotationInfo => "旋轉: X={0:F1} Y={1:F1} Z={2:F1}";
}

/// <summary>
///     path: Game.Command.Stop
/// </summary>
public class StopTextCHT
{
    public string Desc => "關閉服務器";
    public string Usage => "用法: /stop";
    public string ShuttingDown => "服務器正在關閉…";
}

#endregion
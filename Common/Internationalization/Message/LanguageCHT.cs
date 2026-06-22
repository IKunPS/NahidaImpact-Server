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
    public SceneInfoTextCHT SceneInfo { get; } = new();
    public WorldInfoTextCHT WorldInfo { get; } = new();
}

/// <summary>
///     path: Server
/// </summary>
public class ServerTextCHT
{
    public WebTextCHT Web { get; } = new();
    public ServerInfoTextCHT ServerInfo { get; } = new();
    public ConnectionInfoTextCHT ConnectionInfo { get; } = new();
}

/// <summary>
///     path: Word
/// </summary>
public class WordTextCHT
{
    public string Config => "配置文件";
    public string Language => "語言";
    public string GameData => "遊戲數據";
    public string CustomData => "自定義數據";
    public string Database => "數據庫";
    public string Command => "命令";
    public string Dispatch => "全局分發";
    public string Game => "遊戲";
    public string Handbook => "手冊";
    public string NotFound => "未找到";
    public string Error => "錯誤";
    public string DatabaseAccount => "數據庫賬號";
    public string Resource => "資源";
}

#endregion

#region Layer 2

/// <summary>
///     path: Server.Web
/// </summary>
public class WebTextCHT
{
    public string Maintain => "服務器正在維修，請稍後嘗試。";
    public string AccountNotFound => "賬號未找到";
    public string PasswordIncorrect => "密碼錯誤";
    public string LoginFailed => "賬號登錄失敗";
    public string DecryptionFailed => "賬號名解密失敗";
    public string CreateAccountFailed => "創建賬號失敗";
    public string TokenError => "賬號令牌錯誤";
    public string Relogin => "為了賬號安全，請重新登錄";
    public string OK => "OK";
    public string CacheError => "賬號緩存錯誤";
    public string ConnectionFailed => "連接失敗！";
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
    public string LoadingItem => "正在加載 {0}…";
    public string GeneratingItem => "正在生成 {0}…";
    public string RegisterItem => "註冊了 {0} 個 {1}。";
    public string FailedToLoadItem => "加載 {0} 失敗。";
    public string FailedToInitializeItem => "初始化 {0} 失敗。";
    public string FailedToReadItem => "讀取 {0} 失敗，文件{1}";
    public string GeneratedItem => "已生成 {0}。";

    public string LoadedItem => "已加載 {0}。";
    public string LoadedItems => "已加載 {0} 個 {1}。";
    public string ServerRunning => "{0} 服務器正在監聽 {1}";
    public string ServerStarted => "啟動完成！用時 {0}s，輸入 『help』 來獲取命令幫助";
    public string ConfigMissing => "{0} 缺失，請檢查你的資源文件夾: {1}，{2} 可能不能使用。";
    public string SaveDatabase => "已保存數據庫，用時 {0}s";
    public string WaitForAllDone => "現在還不可以進入遊戲，請等待所有項目加載完成後再試";
    public string UnhandledException => "發生未經處理的異常: {0}";
    public string DirNotFound => "{0} 目錄未找到: {1}";
    public string FailedToLoadData => "加載 {0} 數據失敗。";
    public string FailedToLoadFile => "加載 {0} 失敗: {1}";

    public string LoadedScenePoints => "已加載 {0} 個場景傳送點，覆蓋 {1} 個場景";
    public string FileNotFound => "{0} 文件未找到: {1}";
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
    public AccountTextCHT Account { get; } = new();
    public KickTextCHT Kick { get; } = new();
    public SpawnTextCHT Spawn { get; } = new();
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
    public string InvalidArguments => "無效的參數！";
    public string NoPermission => "你沒有權限這麼做！";
    public string CommandNotFound => "未找到命令！輸入 '/help' 來獲取幫助";
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
    public string Desc => "給予玩家物品或角色";
    public string Usage => "用法: /give <itemId|avatarId> [lv<等級>] [x<數量>] [r<精煉>] [c<命座>] [sl<天賦>]\n" +
                           "       /give <artifactId> [lv<等級>] [x<數量>] [<主屬性ID>] [<副屬性ID[,次數]>]...\n" +
                           "       /give <all|weapons|mats|furniture|avatars> [lv<等級>] [x<數量>] [r<精煉>]";
    public string InvalidItemId => "無效的物品ID。";
    public string InvalidAmount => "無效的數量。";
    public string GivenItem => "已給予 {0}x 物品 {1} 給玩家 {2}。";
    public string GivenWeapon => "已給予 {0}x 武器 {1} (等級 {2}, 精煉 {3}) 給玩家 {4}。";
    public string GivenRelic => "已給予 {0}x 聖遺物 {1} (等級 {2}) 給玩家 {3}。";
    public string GivenAvatar => "已給予角色 {0} (等級 {1}) 給玩家 {2}。";
    public string Failed => "無法給予物品 {0}。";
}

/// <summary>
///     path: Game.Command.GiveAll
/// </summary>
public class GiveAllTextCHT
{
    public string Desc => "給予玩家全部指定類型的物品";
    public string Usage => "用法: /give <all|weapons|relics|mats|furniture|avatars> [lv<等級>] [x<數量>] [r<精煉>]";
    public string AvatarsGiven => "已給予 {0} 個角色 (UID {1})。";
    public string WeaponsGiven => "已給予 {0} 把武器 (UID {3})。";
    public string RelicsGiven => "已給予 {0} 件聖遺物 (UID {3})。";
    public string MaterialsGiven => "已給予 {0} 種材料 x{1} (UID {3})。";
    public string FurnitureGiven => "已給予 {0} 件擺設 x{1} (UID {3})。";
    public string ItemsGiven => "已給予 {0} 個物品 (UID {3})。";
    public string AllGiven => "全部物品已給予 UID {0}。";
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
///     path: Game.Command.Spawn
/// </summary>
public class SpawnTextCHT
{
    public string Desc => "生成實體";
    public string Usage => "用法: /spawn <id> [x<數量>] [lv<等級>] [state<狀態>] [ai<aiId>] [<x> <y> <z>]";
    public string InvalidEntityId => "無效的實體ID。";
    public string NoScene => "玩家不在場景中。";
    public string Success => "已生成 {0} 個 {1} (ID: {2}, Lv: {3}) (UID {4})。";
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

#region Layer 3 - Connection / Scene / World log messages

public class ConnectionInfoTextCHT
{
    public string NewConnection => "新連線來自 {0}。";
    public string ConnectionClosed => "連線已關閉";
    public string PacketTooLarge => "封包過大";
    public string FailedToReceive => "接收封包失敗";
    public string ParseError => "封包解析錯誤";
    public string BadDataPackage => "收到損壞封包: got 0x{0}, expect 0x4567";
    public string InvalidFooter => "收到無效封包尾: got 0x{0}, expected 0x89AB";
    public string DummySend => "[Dummy] 發送 Dummy {0}";
    public string NoHandlerFound => "未找到封包 {0} 的處理程序 (0x{1})";
    public string RegionCacheError => "初始化區域緩存時出錯！";
}

public class SceneInfoTextCHT
{
    public string NoTeamManagerSpawn => "玩家 {0} 沒有 TeamManager，無法生成角色";
    public string NoAvatarEntity => "玩家 {0} 沒有當前角色實體可生成";
    public string NoTeamManagerSetup => "玩家 {0} 沒有 TeamManager，無法設定角色";
    public string NullEntity => "嘗試向場景 {0} 添加空實體";
    public string NoAvatarData => "未找到角色數據！請檢查你的 ExcelBinOutput 檔案夾。";
}

public class WorldInfoTextCHT
{
    public string NotifyPlayerInfo => "通知玩家 {0} 有關新玩家 {1} 的資訊";
}

#endregion
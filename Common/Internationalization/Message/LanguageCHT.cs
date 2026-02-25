namespace NahidaImpact.Internationalization.Message;

#region Root

public class LanguageCHT
{
    public GameTextCHT Game { get; } = new();
    public ServerTextCHT Server { get; } = new();
    public WordTextCHT Word { get; } = new(); // a placeholder for the actual word text
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
    public string Rank => "星魂";
    public string Avatar => "角色";
    public string Material => "材料";
    public string Pet => "寵物";
    public string Relic => "遺器";
    public string Equipment => "光錐";
    public string Talent => "行跡";
    public string Banner => "卡池";
    public string Activity => "活動";
    public string CdKey => "兌換碼";
    public string VideoKey => "過場動畫金鑰";
    public string Buff => "祝福";
    public string Miracle => "奇物";
    public string Unlock => "奢侈品";
    public string TrainParty => "派對車廂";

    // server info
    public string Config => "配置文件";
    public string Language => "語言";
    public string Log => "日誌";
    public string GameData => "遊戲數據";
    public string Cache => "資源緩存";
    public string CustomData => "自定義數據";
    public string Database => "數據庫";
    public string Command => "命令";
    public string SSL => "SSL";
    public string Ec2b => "Ec2b";
    public string SdkServer => "Web服務器";
    public string Handler => "包處理器";
    public string Dispatch => "全局分發";
    public string Game => "遊戲";
    public string Handbook => "手冊";
    public string NotFound => "未找到";
    public string Error => "錯誤";
    public string FloorInfo => "區域文件";
    public string FloorGroupInfo => "區域組文件";
    public string FloorMissingResult => "傳送與世界生成";
    public string FloorGroupMissingResult => "傳送、怪物戰鬥與世界生成";
    public string Mission => "任務";
    public string MissionInfo => "任務文件";
    public string SubMission => "子任務";
    public string SubMissionInfo => "子任務文件";
    public string MazeSkill => "角色秘技";
    public string MazeSkillInfo => "角色秘技文件";
    public string Dialogue => "模擬宇宙事件";
    public string DialogueInfo => "模擬宇宙事件文件";
    public string Performance => "劇情操作";
    public string PerformanceInfo => "劇情操作文件";
    public string RogueChestMap => "模擬宇宙地圖";
    public string RogueChestMapInfo => "模擬宇宙地圖文件";
    public string ChessRogueRoom => "模擬宇宙DLC";
    public string ChessRogueRoomInfo => "模擬宇宙DLC文件";
    public string SummonUnit => "秘技生成";
    public string SummonUnitInfo => "秘技生成文件";
    public string RogueTournRoom => "差分宇宙";
    public string RogueTournRoomInfo => "差分宇宙房間文件";
    public string TypesOfRogue => "類型的模擬宇宙";
    public string RogueMagicRoom => "不可知域";
    public string RogueMagicRoomInfo => "不可知域房間文件";
    public string RogueDiceSurface => "骰面效果";
    public string RogueDiceSurfaceInfo => "骰面效果文件";
    public string AdventureModifier => "AdventureModifier";
    public string AdventureModifierInfo => "AdventureModifier文件";
    public string RogueMapGen => "RogueMapGen文件";
    public string RogueMiracleGroup => "RogueMiracleGroup文件";
    public string RogueMiracleEffectGen => "RogueMiracleEffectGen文件";

    public string DatabaseAccount => "數據庫賬號";
    public string Tutorial => "教程";
}

#endregion

#region Layer 2

#region GameText

/// <summary>
///     path: Game.Command
/// </summary>
public class CommandTextCHT
{
    public NoticeTextCHT Notice { get; } = new();

    public GenderTextCHT Gender { get; } = new();
    public AvatarTextCHT Avatar { get; } = new();
    public AnnounceTextCHT Announce { get; } = new();
    public BanTextCHT Ban { get; } = new();
    public GiveTextCHT Give { get; } = new();
    public GiveAllTextCHT GiveAll { get; } = new();
    public LineupTextCHT Lineup { get; } = new();
    public HelpTextCHT Help { get; } = new();
    public KickTextCHT Kick { get; } = new();
    public MissionTextCHT Mission { get; } = new();
    public RelicTextCHT Relic { get; } = new();
    public ReloadTextCHT Reload { get; } = new();
    public RogueTextCHT Rogue { get; } = new();
    public SceneTextCHT Scene { get; } = new();
    public UnlockAllTextCHT UnlockAll { get; } = new();
    public MailTextCHT Mail { get; } = new();
    public RaidTextCHT Raid { get; } = new();
    public AccountTextCHT Account { get; } = new();
    public UnstuckTextCHT Unstuck { get; } = new();
    public SetlevelTextCHT Setlevel { get; } = new();
    public PermissionTextCHT Permission { get; } = new();
}

#endregion

#region ServerText

/// <summary>
///     path: Server.Web
/// </summary>
public class WebTextCHT
{
    public string Maintain => "服務器正在維修, 請稍後嘗試。";
}

/// <summary>
///     path: Server.ServerInfo
/// </summary>
public class ServerInfoTextCHT
{
    public string Shutdown => "關閉中…";
    public string CancelKeyPressed => "已按下取消鍵 (Ctrl + C), 服務器即將關閉…";
    public string StartingServer => "正在啟動 NahidaImpact";
    public string CurrentVersion => "當前服務端支援的版本: {0}";
    public string InvalidVersion => "目前為不受支援的遊戲版本 {0}\n請更新遊戲到 {1}";
    public string LoadingItem => "正在加載 {0}…";
    public string GeneratingItem => "正在生成 {0}…";
    public string WaitingItem => "正在等待進程 {0} 完成…";
    public string RegisterItem => "註冊了 {0} 個 {1}。";
    public string FailedToLoadItem => "加載 {0} 失敗。";
    public string NewClientSecretKey => "客戶端密鑰不存在, 正在生成新的客戶端密鑰。";
    public string FailedToInitializeItem => "初始化 {0} 失敗。";
    public string FailedToReadItem => "讀取 {0} 失敗, 文件{1}";
    public string GeneratedItem => "已生成 {0}。";
    public string LoadedItem => "已加載 {0}。";
    public string LoadedItems => "已加載 {0} 個 {1}。";
    public string ServerRunning => "{0} 服務器正在監聽 {1}";
    public string ServerStarted => "啟動完成!用時 {0}s, 擊敗了99%的用戶, 輸入 『help』 來獲取命令幫助"; // 玩梗, 考慮英語版本將其本土化
    public string MissionEnabled => "任務系統已啟用, 此功能仍在開發中, 且可能不會按預期工作, 如果遇見任何bug, 請匯報給開發者。";
    public string KeyStoreError => "SSL證書不存在, 已關閉SSL功能。";
    public string CacheLoadSkip => "已跳過緩存加載。";

    public string ConfigMissing => "{0} 缺失, 請檢查你的資源文件夾: {1}, {2} 可能不能使用。";
    public string UnloadedItems => "卸載了所有 {0}。";
    public string SaveDatabase => "已保存數據庫, 用時 {0}s";
    public string WaitForAllDone => "現在還不可以進入遊戲, 請等待所有項目加載完成後再試";

    public string UnhandledException => "發生未經處理的異常: {0}";
}

#endregion

#endregion

#region Layer 3

#region CommandText

/// <summary>
///     path: Game.Command.Notice
/// </summary>
public class NoticeTextCHT
{
    public string PlayerNotFound => "未找到玩家!";
    public string PlayerNotInit => "玩家未初始化。";
    public string InvalidArguments => "無效的參數!";
    public string NoPermission => "你沒有權限這麽做!";
    public string CommandNotFound => "未找到命令! 輸入 '/help' 來獲取幫助";
    public string TargetOffline => "目標 {0}({1}) 離線了!清除當前目標";
    public string TargetFound => "找到目標 {0}({1}), 下一次命令將默認對其執行";
    public string TargetNotFound => "未找到目標 {0}!";
    public string InternalError => "在處理命令時發生了內部錯誤: {0}!";
}

/// <summary>
///     path: Game.Command.Gender
/// </summary>
public class GenderTextCHT
{
    public string Desc => "切換主角的性別";
    public string Usage => "用法: /gender [man/woman]";
    public string GenderNotSpecified => "性別不存在!";
    public string GenderChanged => "性別已更改!";
}

/// <summary>
///     path: Game.Command.UnlockAll
/// </summary>
public class UnlockAllTextCHT
{
    public string Desc =>
        "解鎖所有在類別內的對象\n" +
        "使用 /unlockall mission 以完成所有任務, 使用後會被踢出, 重新登錄後可能會被教程卡住, 請謹慎使用\n" +
        "使用 /unlockall tutorial 以解鎖所有教程, 使用後會被踢出, 用於部分界面卡住無法行動的情況\n" +
        "使用 /unlockall rogue 以解鎖所有類型模擬宇宙, 使用後會被踢出, 建議與 /unlockall tutorial 搭配使用以獲取更好效果";

    public string Usage => "用法: /unlockall [mission/tutorial/rogue]";
    public string UnlockedAll => "已解鎖/完成所有{0}!";
}

/// <summary>
///     path: Game.Command.Avatar
/// </summary>
public class AvatarTextCHT
{
    public string Desc => "設定玩家已有角色的屬性, -1意為所有已擁有角色";

    public string Usage =>
        "用法: /avatar talent [角色ID/-1] [行跡等級]\n" +
        "用法: /avatar rank [角色ID/-1] [星魂]\n" +
        "用法: /avatar level [角色ID/-1] [角色等級]";
    public string InvalidLevel => "{0}等級無效!";
    public string AllAvatarsLevelSet => "已將全部角色 {0}等級設置為 {1}.";
    public string AvatarLevelSet => "已將 {0} 角色 {1}等級設置為 {2}.";
    public string AvatarNotFound => "角色不存在!";

    public string InvalidId => "無效的角色ID。";
    public string AddFailed => "添加角色 {0} 失敗。角色可能已存在或ID無效。";
    public string AddSuccess => "成功添加角色 {0} (Lv.{1}) 給玩家 {2}。";
    public string AddAllSuccess => "成功添加 {0} 個角色 (Lv.{1}) 給玩家 {2}。";
}

/// <summary>
///     path: Game.Command.Give
/// </summary>
public class GiveTextCHT
{
    public string Desc => "給予玩家物品";
    public string Usage => "用法: /give [物品ID] l[等級] x[數量] r[疊影]";
    public string ItemNotFound => "未找到物品!";
    public string GiveItem => "給予 @{0} {1} 個物品 {2}.";
}

/// <summary>
///     path: Game.Command.GiveAll
/// </summary>
public class GiveAllTextCHT
{
    public string Desc => "給予玩家全部指定類型的物品";
    public string Usage =>
        "用法: /giveall avatar r[星魂] l[等級]\n" +
        "用法: /giveall material x[數量]\n" +
        "用法: /giveall equipment r[叠影] l[等級] x[數量]\n" +
        "用法: /giveall relic l<等級> x<數量>\n" +
        "用法: /giveall unlock\n" +
        "用法: /giveall train\n" +
        "用法: /giveall path";
    public string GiveAllItems => "已給予所有 {0}, 各 {1} 個.";
}

/// <summary>
///     path: Game.Command.Lineup
/// </summary>
public class LineupTextCHT
{
    public string Desc => "管理玩家的隊伍資訊";
    public string Usage => 
        "用法: /lineup mp\n" +
        "用法: /lineup sp\n" +
        "用法: /lineup heal";
    public string GainedMp => "成功恢復秘技點!";
    public string GainedSp => "成功恢復能量!";
    public string HealedAllAvatars => "成功治愈當前隊伍中的所有角色!";
}

/// <summary>
///     path: Game.Command.Help
/// </summary>
public class HelpTextCHT
{
    public string Desc => "顯示幫助信息";
    public string Usage =>
        "用法: /help\n" +
        "用法: /help [命令]";
    public string Commands => "命令: ";
    public string CommandPermission => "所需權限: ";
    public string CommandAlias => "命令別名: ";
}

/// <summary>
///     path: Game.Command.Kick
/// </summary>
public class KickTextCHT
{
    public string Desc => "踢出玩家";
    public string Usage => "用法: /kick";
    public string PlayerKicked => "玩家 {0} 已被踢出!";
}

/// <summary>
///     path: Game.Command.Mission
/// </summary>
public class MissionTextCHT
{
    public string Desc =>
        "管理玩家的任務\n" +
        "使用 pass 完成當前正在進行的所有任務, 此命令易造成嚴重卡頓, 請盡量使用 /mission finish 替代\n" +
        "使用 finish [子任務ID] 完成指定子任務, 請流覽 handbook 來獲取子任務ID\n" +
        "使用 finishmain [主任務ID] 完成指定主任務, 請流覽 handbook 來獲取主任務ID\n" +
        "使用 running [-all] 獲取正在追蹤的任務, 增加'-all'則顯示所有正在進行的任務以及可能卡住的任務, 使用後可能會出現較長任務清單, 請注意甄別\n" +
        "使用 reaccept [主任務ID] 可重新進行指定主任務, 請流覽 handbook 來獲取主任務ID";

    public string Usage =>
        "用法: /mission pass\n" +
        "用法: /mission finish [子任務]\n" +
        "用法: /mission running [-all]\n" +
        "用法: /mission reaccept [主任務ID]\n" +
        "用法: /mission finishmain [主任務ID]";
    public string AllMissionsFinished => "所有任務已完成!";
    public string AllRunningMissionsFinished => "共 {0} 個進行中的任務已完成!";
    public string MissionFinished => "任務 {0} 已完成!";
    public string InvalidMissionId => "無效的任務ID!";
    public string NoRunningMissions => "沒有正在進行的任務!";
    public string RunningMissions => "正在進行的任務: ";
    public string PossibleStuckMissions => "可能卡住的任務: ";
    public string MainMission => "主任務";
    public string MissionReAccepted => "重新接受任務 {0}.";
}

/// <summary>
///     path: Game.Command.Relic
/// </summary>
public class RelicTextCHT
{
    public string Desc => "管理玩家的遺器, 等級限製: 1 ≤ 等級 ≤ 9999";
    public string Usage => "用法: /relic [遺器ID] [主詞條ID] [ID1:等級] [ID2:等級]  l[等級] x[數量]";
    public string RelicNotFound => "遺器不存在!";
    public string InvalidMainAffixId => "主詞條ID無效!";
    public string InvalidSubAffixId => "副詞條ID無效!";
    public string RelicGiven => "給予玩家 @{0} {1} 個遺器 {2}, 主詞條 {3}.";
}

/// <summary>
///     path: Game.Command.Reload
/// </summary>
public class ReloadTextCHT
{
    public string Desc => "重新加載指定的配置";
    public string Usage => "用法: /reload [banner/activity]";
    public string ConfigReloaded => "配置 {0} 已重新加載!";
}

/// <summary>
///     path: Game.Command.Rogue
/// </summary>
public class RogueTextCHT
{
    public string Desc => "管理模擬宇宙數據, -1意為所有已擁有祝福, buff來獲取祝福, enhance強化祝福";

    public string Usage =>
        "用法: /rogue money [宇宙碎片數量]\n" +
        "用法: /rogue buff [祝福ID/-1]\n" +
        "用法: /rogue miracle [奇物ID]\n" +
        "用法: /rogue enhance [祝福ID/-1]\n" +
        "用法: /rogue unstuck - 脫離事件";
    public string PlayerGainedMoney => "已獲得 {0} 宇宙碎片.";
    public string PlayerGainedAllItems => "已獲得所有{0}.";
    public string PlayerGainedItem => "已獲得{0} {1}.";
    public string PlayerEnhancedBuff => "已強化祝福 {0}.";
    public string PlayerEnhancedAllBuffs => "已強化所有祝福.";
    public string PlayerUnstuck => "已脫離事件.";
    public string NotFoundItem => "未找到 {0}!";
    public string PlayerNotInRogue => "玩家不在模擬宇宙中!";
}

/// <summary>
///     path: Game.Command.Scene
/// </summary>
public class SceneTextCHT
{
    public string Desc =>
        "管理玩家場景\n" +
        "使用 PlaneId 預設進入指定場景\n" +
        "使用 group 來獲取組, 使用 prop 來設置道具狀態, 在 PropStateEnum 獲取狀態列表\n" +
        "使用 unlockall 來解鎖場景內所有道具(open狀態), 可能導致遊戲加載卡條, 使用 /scene reset 解決\n" +
        "使用 reload 來重新加載當前場景, 並回到初始位置\n" +
        "使用 reset 來重置指定場景所有道具狀態";
    public string Usage =>
        "用法: /scene [entryId]\n" +
        "用法: /scene cur\n" +
        "用法: /scene reload\n" +
        "用法: /scene group\n" +
        "用法: /scene unlockall\n" +
        "用法: /scene reset [floorId]" +
        "用法: /scene prop [組ID] [道具ID] [狀態]\n" +
        "用法: /scene remove [實體ID]\n";
    public string LoadedGroups => "已加載組: {0}.";
    public string PropStateChanged => "道具: {0} 的狀態已設置為 {1}.";
    public string PropNotFound => "未找到道具!";
    public string EntityRemoved => "實體 {0} 已被移除.";
    public string EntityNotFound => "未找到實體!";
    public string AllPropsUnlocked => "所有道具已解鎖!";
    public string SceneChanged => "已進入場景 {0}.";
    public string SceneReloaded => "場景已重新加載!";
    public string SceneReset => "已重置場景 {0} 中所有道具狀態!";
    public string CurrentScene => "當前場景Entry Id: {0}, Plane Id: {1}, Floor Id: {2}.";
}

/// <summary>
///     path: Game.Command.Mail
/// </summary>
public class MailTextCHT
{
    public string Desc => "發送郵件";
    public string Usage => "用法: /mail [發送名稱] [標題] [內容] [ID1:數量,ID2:數量]";
    public string MailSent => "郵件已發送!";
}

/// <summary>
///     path: Game.Command.Raid
/// </summary>
public class RaidTextCHT
{
    public string Desc => "管理玩家的任務臨時場景";
    public string Usage => "用法: /raid leave";
    public string Leaved => "已離開臨時場景!";
}

/// <summary>
///     path: Game.Command.Account
/// </summary>
public class AccountTextCHT
{
    public string Desc => "管理資料庫帳號";
    public string Usage =>
        "用法: /account create [用户名] [UID] [密碼]\n" +
        "用法: /account delete [UID]";
    public string InvalidUid => "UID無效!";
    public string InvalidAccount => "帳號 {0} 無效!";
    public string CreateSuccess => "賬號 {0} 創建成功!";
    public string DeleteSuccess => "賬號 {0} 刪除成功!";
}

/// <summary>
///     path: Game.Command.Announce
/// </summary>
public class AnnounceTextCHT
{
    public string Desc => "發送彈窗公告";
    public string Usage => "用法: /announce [Text] [Color]";
    public string SendSuccess => "發送成功!";
}

/// <summary>
///     path: Game.Command.Ban
/// </summary>
public class BanTextCHT
{
    public string Desc => "封禁或解封用户";
    public string Usage => "用法: /ban [add/delete]";
    public string BanSuccess => "帳號已封禁!";
    public string UnBanSuccess => "帳號已解封!";
}

/// <summary>
///     path: Game.Command.Unstuck
/// </summary>
public class UnstuckTextCHT
{
    public string Desc => "將玩家傳送回默認場景";
    public string Usage => "用法: /unstuck [UID]";
    public string UnstuckSuccess => "已成功將該玩家傳送回默認場景";
    public string UidNotExist => "該UID不存在!";
    public string PlayerIsOnline => "該玩家目前在線上!";
}

/// <summary>
///     path: Game.Command.Setlevel
/// </summary>
public class SetlevelTextCHT
{
    public string Desc => "設定玩家等級";
    public string Usage => "用法: /setlevel [等級]";
    public string SetlevelSuccess => "等級設定成功!";
}

/// <summary>
///     path: Game.Command.Permission
/// </summary>
public class PermissionTextCHT
{
    public string Desc => "管理玩家權限";
    public string Usage =>
        "用法: /permission add [權限]\n" +
        "用法: /permission remove [權限]\n" +
        "用法: /permission clean [權限]";
    public string InvalidPerm => "權限 {0} 不存在!";
    public string Added => "已添加權限 {0} 到玩家 {1}!";
    public string Removed => "已移除玩家 {0} 的權限 {1}!";
    public string Cleaned => "已清除玩家 {0} 的所有權限!";
}

#endregion

#endregion
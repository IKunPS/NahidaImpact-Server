namespace NahidaImpact.Internationalization.Message;

#region Root

public class LanguageCHS
{
    public GameTextCHS Game { get; } = new();
    public ServerTextCHS Server { get; } = new();
    public WordTextCHS Word { get; } = new(); // a placeholder for the actual word text
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
    public string Rank => "星魂";
    public string Avatar => "角色";
    public string Material => "材料";
    public string Pet => "宠物";
    public string Relic => "遗器";
    public string Equipment => "光锥";
    public string Talent => "行迹";
    public string Banner => "卡池";
    public string Activity => "活动";
    public string CdKey => "兑换码";
    public string VideoKey => "过场动画密钥";
    public string Buff => "祝福";
    public string Miracle => "奇物";
    public string Unlock => "奢侈品";
    public string TrainParty => "派对车厢";

    // server info
    public string Config => "配置文件";
    public string Language => "语言";
    public string Log => "日志";
    public string GameData => "游戏数据";
    public string Cache => "资源缓存";
    public string CustomData => "自定义数据";
    public string Database => "数据库";
    public string Command => "命令";
    public string SSL => "SSL";
    public string Ec2b => "Ec2b";
    public string SdkServer => "Web服务器";
    public string Handler => "包处理器";
    public string Dispatch => "全局分发";
    public string Game => "游戏";
    public string Handbook => "手册";
    public string NotFound => "未找到";
    public string Error => "错误";
    public string FloorInfo => "区域文件";
    public string FloorGroupInfo => "区域组文件";
    public string FloorMissingResult => "传送与世界生成";
    public string FloorGroupMissingResult => "传送、怪物战斗与世界生成";
    public string Mission => "任务";
    public string MissionInfo => "任务文件";
    public string SubMission => "子任务";
    public string SubMissionInfo => "子任务文件";
    public string MazeSkill => "角色秘技";
    public string MazeSkillInfo => "角色秘技文件";
    public string Dialogue => "模拟宇宙事件";
    public string DialogueInfo => "模拟宇宙事件文件";
    public string Performance => "剧情操作";
    public string PerformanceInfo => "剧情操作文件";
    public string RogueChestMap => "模拟宇宙地图";
    public string RogueChestMapInfo => "模拟宇宙地图文件";
    public string ChessRogueRoom => "模拟宇宙DLC";
    public string ChessRogueRoomInfo => "模拟宇宙DLC文件";
    public string SummonUnit => "秘技生成";
    public string SummonUnitInfo => "秘技生成文件";
    public string RogueTournRoom => "差分宇宙";
    public string RogueTournRoomInfo => "差分宇宙房间文件";
    public string TypesOfRogue => "类型的模拟宇宙";
    public string RogueMagicRoom => "不可知域";
    public string RogueMagicRoomInfo => "不可知域房间文件";
    public string RogueDiceSurface => "骰面效果";
    public string RogueDiceSurfaceInfo => "骰面效果文件";
    public string AdventureModifier => "AdventureModifier";
    public string AdventureModifierInfo => "AdventureModifier文件";
    public string RogueMapGen => "RogueMapGen文件";
    public string RogueMiracleGroup => "RogueMiracleGroup文件";
    public string RogueMiracleEffectGen => "RogueMiracleEffectGen文件";

    public string DatabaseAccount => "数据库账号";
    public string Tutorial => "教程";
}

#endregion

#region Layer 2

#region GameText

/// <summary>
///     path: Game.Command
/// </summary>
public class CommandTextCHS
{
    public NoticeTextCHS Notice { get; } = new();

    public GenderTextCHS Gender { get; } = new();
    public AvatarTextCHS Avatar { get; } = new();
    public AnnounceTextCHS Announce { get; } = new();
    public BanTextCHS Ban { get; } = new();
    public GiveTextCHS Give { get; } = new();
    public GiveAllTextCHS GiveAll { get; } = new();
    public LineupTextCHS Lineup { get; } = new();
    public HelpTextCHS Help { get; } = new();
    public KickTextCHS Kick { get; } = new();
    public MissionTextCHS Mission { get; } = new();
    public RelicTextCHS Relic { get; } = new();
    public ReloadTextCHS Reload { get; } = new();
    public RogueTextCHS Rogue { get; } = new();
    public SceneTextCHS Scene { get; } = new();
    public UnlockAllTextCHS UnlockAll { get; } = new();
    public MailTextCHS Mail { get; } = new();
    public RaidTextCHS Raid { get; } = new();
    public AccountTextCHS Account { get; } = new();
    public UnstuckTextCHS Unstuck { get; } = new();
    public SetlevelTextCHS Setlevel { get; } = new();
    public PermissionTextCHS Permission { get; } = new();
}

#endregion

#region ServerText

/// <summary>
///     path: Server.Web
/// </summary>
public class WebTextCHS
{
    public string Maintain => "服务器正在维修, 请稍后尝试。";
}

/// <summary>
///     path: Server.ServerInfo
/// </summary>
public class ServerInfoTextCHS
{
    public string Shutdown => "关闭中…";
    public string CancelKeyPressed => "已按下取消键 (Ctrl + C), 服务器即将关闭…";
    public string StartingServer => "正在启动 NahidaImpact";
    public string CurrentVersion => "当前服务端支持的版本: {0}";
    public string InvalidVersion => "当前为不受支持的游戏版本 {0}\n请更新游戏到 {1}";
    public string LoadingItem => "正在加载 {0}…";
    public string GeneratingItem => "正在生成 {0}…";
    public string WaitingItem => "正在等待进程 {0} 完成…";
    public string RegisterItem => "注册了 {0} 个 {1}。";
    public string FailedToLoadItem => "加载 {0} 失败。";
    public string NewClientSecretKey => "客户端密钥不存在, 正在生成新的客户端密钥。";
    public string FailedToInitializeItem => "初始化 {0} 失败。";
    public string FailedToReadItem => "读取 {0} 失败, 文件{1}";
    public string GeneratedItem => "已生成 {0}。";
    public string LoadedItem => "已加载 {0}。";
    public string LoadedItems => "已加载 {0} 个 {1}。";
    public string ServerRunning => "{0} 服务器正在监听 {1}";
    public string ServerStarted => "启动完成!用时 {0}s, 击败了99%的用户, 输入 ‘help’ 来获取命令帮助"; // 玩梗, 考虑英语版本将其本土化
    public string MissionEnabled => "任务系统已启用, 此功能仍在开发中, 且可能不会按预期工作, 如果遇见任何bug, 请汇报给开发者。";
    public string KeyStoreError => "SSL证书不存在, 已关闭SSL功能。";
    public string CacheLoadSkip => "已跳过缓存加载。";

    public string ConfigMissing => "{0} 缺失, 请检查你的资源文件夹: {1}, {2} 可能不能使用。";
    public string UnloadedItems => "卸载了所有 {0}。";
    public string SaveDatabase => "已保存数据库, 用时 {0}s";
    public string WaitForAllDone => "现在还不可以进入游戏, 请等待所有项目加载完成后再试";

    public string UnhandledException => "发生未经处理的异常: {0}";
}

#endregion

#endregion

#region Layer 3

#region CommandText

/// <summary>
///     path: Game.Command.Notice
/// </summary>
public class NoticeTextCHS
{
    public string PlayerNotFound => "未找到玩家!";
    public string PlayerNotInit => "玩家未初始化。";
    public string InvalidArguments => "无效的参数!";
    public string NoPermission => "你没有权限这么做!";
    public string CommandNotFound => "未找到命令! 输入 '/help' 来获取帮助";
    public string TargetOffline => "目标 {0}({1}) 离线了!清除当前目标";
    public string TargetFound => "找到目标 {0}({1}), 下一次命令将默认对其执行";
    public string TargetNotFound => "未找到目标 {0}!";
    public string InternalError => "在处理命令时发生了内部错误: {0}!";
}

/// <summary>
///     path: Game.Command.Gender
/// </summary>
public class GenderTextCHS
{
    public string Desc => "切换主角的性别";
    public string Usage => "用法: /gender [man/woman]";
    public string GenderNotSpecified => "性别不存在!";
    public string GenderChanged => "性别已更改!";
}

/// <summary>
///     path: Game.Command.UnlockAll
/// </summary>
public class UnlockAllTextCHS
{
    public string Desc =>
        "解锁所有在类别内的对象\n" +
        "使用 /unlockall mission 以完成所有任务, 使用后会被踢出, 重新登录后可能会被教程卡住, 请谨慎使用\n" +
        "使用 /unlockall tutorial 以解锁所有教程, 使用后会被踢出, 用于部分界面卡住无法行动的情况\n" +
        "使用 /unlockall rogue 以解锁所有类型模拟宇宙, 使用后会被踢出, 建议与 /unlockall tutorial 搭配使用以获取更好效果";
    public string Usage => "用法: /unlockall [mission/tutorial/rogue]";
    public string UnlockedAll => "已解锁/完成所有{0}!";
}

/// <summary>
///     path: Game.Command.Avatar
/// </summary>
public class AvatarTextCHS
{
    public string Desc => "设定玩家已有角色的属性, -1为所有已拥有角色";
    public string Usage =>
        "用法: /avatar talent [角色ID/-1] [行迹等级]\n" +
        "用法: /avatar rank [角色ID/-1] [星魂]\n" +
        "用法: /avatar level [角色ID/-1] [角色等级]";
    public string InvalidLevel => "{0} 等级无效!";
    public string AllAvatarsLevelSet => "已将全部角色 {0} 等级设置为 {1}.";
    public string AvatarLevelSet => "已将 {0} 角色 {1} 等级设置为 {2}.";
    public string AvatarNotFound => "角色不存在!";

    public string InvalidId => "无效的角色ID。";
    public string AddFailed => "添加角色 {0} 失败。角色可能已存在或ID无效。";
    public string AddSuccess => "成功添加角色 {0} (Lv.{1}) 给玩家 {2}。";
    public string AddAllSuccess => "成功添加 {0} 个角色 (Lv.{1}) 给玩家 {2}。";
}

/// <summary>
///     path: Game.Command.Give
/// </summary>
public class GiveTextCHS
{
    public string Desc => "给予玩家物品";
    public string Usage => "用法: /give [物品ID] l[等级] x[数量] r[叠影]";
    public string ItemNotFound => "未找到物品!";
    public string GiveItem => "已给予 {0} {1} 个物品 {2}.";
}

/// <summary>
///     path: Game.Command.GiveAll
/// </summary>
public class GiveAllTextCHS
{
    public string Desc => "给予玩家全部指定类型的物品";
    public string Usage =>
        "用法: /giveall avatar r[星魂] l[等级]\n" +
        "用法: /giveall material x[数量]\n" +
        "用法: /giveall equipment r[叠影] l[等级] x[数量]\n" +
        "用法: /giveall relic x[数量]\n" +
        "用法: /giveall unlock\n" +
        "用法: /giveall train\n" +
        "用法: /giveall path";
    public string GiveAllItems => "已给予所有 {0}, 各 {1} 个.";
}

/// <summary>
///     path: Game.Command.Lineup
/// </summary>
public class LineupTextCHS
{
    public string Desc => "管理玩家的队伍信息";
    public string Usage =>
        "用法: /lineup mp\n" +
        "用法: /lineup sp\n" +
        "用法: /lineup heal";
    public string GainedMp => "成功恢复秘技点!";
    public string GainedSp => "成功恢复能量!";
    public string HealedAllAvatars => "成功治愈当前队伍中的所有角色!";
}

/// <summary>
///     path: Game.Command.Help
/// </summary>
public class HelpTextCHS
{
    public string Desc => "显示帮助信息";
    public string Usage =>
        "用法: /help\n" +
        "用法: /help [命令]";
    public string Commands => "命令: ";
    public string CommandPermission => "所需权限: ";
    public string CommandAlias => "命令别名: ";
}

/// <summary>
///     path: Game.Command.Kick
/// </summary>
public class KickTextCHS
{
    public string Desc => "踢出玩家";
    public string Usage => "用法: /kick";
    public string PlayerKicked => "玩家 {0} 已被踢出!";
}

/// <summary>
///     path: Game.Command.Mission
/// </summary>
public class MissionTextCHS
{
    public string Desc =>
        "管理玩家的任务\n" +
        "使用 pass 完成当前正在进行的所有任务, 此命令易造成严重卡顿, 请尽量使用 /mission finish 替代\n" +
        "使用 finish [子任务ID] 完成指定子任务, 请浏览 handbook 来获取子任务ID\n" +
        "使用 finishmain [主任务ID] 完成指定主任务, 请浏览 handbook 来获取主任务ID\n" +
        "使用 running [-all] 获取正在追踪的任务, 增加'-all'则显示所有正在进行的任务以及可能卡住的任务, 使用后可能会出现较长任务列表, 请注意甄别\n" +
        "使用 reaccept [主任务ID] 可重新进行指定主任务, 请浏览 handbook 来获取主任务ID";
    public string Usage =>
        "用法: /mission pass\n" +
        "用法: /mission finish [子任务ID]\n" +
        "用法: /mission running [-all]\n" +
        "用法: /mission reaccept [主任务ID]\n" +
        "用法: /mission finishmain [主任务ID]";
    public string AllMissionsFinished => "所有任务已完成!";
    public string AllRunningMissionsFinished => "共 {0} 个进行中的任务已完成!";
    public string MissionFinished => "任务 {0} 已完成!";
    public string InvalidMissionId => "无效的任务ID!";
    public string NoRunningMissions => "没有正在进行的任务!";
    public string RunningMissions => "正在进行的任务: ";
    public string PossibleStuckMissions => "可能卡住的任务: ";
    public string MainMission => "主任务";
    public string MissionReAccepted => "重新接受任务 {0}.";
}

/// <summary>
///     path: Game.Command.Relic
/// </summary>
public class RelicTextCHS
{
    public string Desc => "管理玩家的遗器, 等级限制: 1 ≤ 等级 ≤ 9999";
    public string Usage => "用法: /relic [遗器ID] [主词条ID] [ID1:等级] [ID2:等级] l[等级] x[数量]";
    public string RelicNotFound => "遗器不存在!";
    public string InvalidMainAffixId => "主词条ID无效!";
    public string InvalidSubAffixId => "副词条ID无效!";
    public string RelicGiven => "给予玩家 {0} {1} 个遗器 {2}.";
}

/// <summary>
///     path: Game.Command.Reload
/// </summary>
public class ReloadTextCHS
{
    public string Desc => "重新加载指定的配置";
    public string Usage => "用法: /reload [banner/activity]";
    public string ConfigReloaded => "配置 {0} 已重新加载!";
}

/// <summary>
///     path: Game.Command.Rogue
/// </summary>
public class RogueTextCHS
{
    public string Desc => "管理模拟宇宙数据, -1意为所有已拥有祝福, buff获取祝福, enhance强化祝福";
    public string Usage =>
        "用法: /rogue money [宇宙碎片数量]\n" +
        "用法: /rogue buff [祝福ID/-1]\n" +
        "用法: /rogue miracle [奇物ID]\n" +
        "用法: /rogue enhance [祝福ID/-1]\n" +
        "用法: /rogue unstuck - 脱离事件";
    public string PlayerGainedMoney => "已获得 {0} 宇宙碎片.";
    public string PlayerGainedAllItems => "已获得所有{0}.";
    public string PlayerGainedItem => "已获得{0} {1}.";
    public string PlayerEnhancedBuff => "已强化祝福 {0}.";
    public string PlayerEnhancedAllBuffs => "已强化所有祝福.";
    public string PlayerUnstuck => "已脱离事件.";
    public string NotFoundItem => "未找到 {0}!";
    public string PlayerNotInRogue => "玩家不在模拟宇宙中!";
}

/// <summary>
///     path: Game.Command.Scene
/// </summary>
public class SceneTextCHS
{
    public string Desc =>
        "管理玩家场景\n" +
        "使用 PlaneId 默认进入指定场景\n" +
        "使用 group 来获取组, 使用 prop 来设置道具状态, 在 PropStateEnum 获取状态列表\n" +
        "使用 unlockall 来解锁场景内所有道具(open状态), 可能导致游戏加载卡条, 使用 /scene reset 解决\n" +
        "使用 reload 来重新加载当前场景, 并回到初始位置\n" +
        "使用 reset 来重置指定场景所有道具状态";
    public string Usage =>
        "用法: /scene [PlaneId]\n" +
        "用法: /scene cur\n" +
        "用法: /scene reload\n" +
        "用法: /scene group\n" +
        "用法: /scene unlockall\n" +
        "用法: /scene reset [PlaneId]" +
        "用法: /scene prop [组ID] [道具ID] [状态]\n" +
        "用法: /scene remove [实体ID]\n";

    public string LoadedGroups => "已加载组: {0}.";
    public string PropStateChanged => "道具: {0} 的状态已设置为 {1}.";
    public string PropNotFound => "未找到道具!";
    public string EntityRemoved => "实体 {0} 已被移除.";
    public string EntityNotFound => "未找到实体!";
    public string AllPropsUnlocked => "所有道具已解锁!";
    public string SceneChanged => "已进入场景 {0}.";
    public string SceneReloaded => "场景已重新加载!";
    public string SceneReset => "已重置场景 {0} 中所有道具状态!";
    public string CurrentScene => "当前场景 EntryId: {0}, PlaneId: {1}, FloorId: {2}.";
}

/// <summary>
///     path: Game.Command.Mail
/// </summary>
public class MailTextCHS
{
    public string Desc => "发送邮件";
    public string Usage => "用法: /mail [发送名称] [标题] [内容] [ID1:数量,ID2:数量]";
    public string MailSent => "邮件已发送!";
}

/// <summary>
///     path: Game.Command.Raid
/// </summary>
public class RaidTextCHS
{
    public string Desc => "管理玩家的任务临时场景";
    public string Usage => "用法: /raid leave";
    public string Leaved => "已离开临时场景!";
}

/// <summary>
///     path: Game.Command.Account
/// </summary>
public class AccountTextCHS
{
    public string Desc => "管理数据库账号";
    public string Usage =>
        "用法: /account create [用户名] [UID] [密码]\n" +
        "用法: /account delete [UID]";
    public string InvalidUid => "UID无效!";
    public string InvalidAccount => "账号 {0} 无效!";
    public string CreateSuccess => "账号 {0} 创建成功!";
    public string DeleteSuccess => "账号 {0} 删除成功!";
}

/// <summary>
///     path: Game.Command.Announce
/// </summary>
public class AnnounceTextCHS
{
    public string Desc => "发送弹窗公告";
    public string Usage => "用法: /announce [Text] [Color]";
    public string SendSuccess => "发送成功!";
}

/// <summary>
///     path: Game.Command.Ban
/// </summary>
public class BanTextCHS
{
    public string Desc => "封禁或解封用户";
    public string Usage => "用法: /ban [add/delete]";
    public string BanSuccess => "账号已封禁!";
    public string UnBanSuccess => "账号已解封!";
}

/// <summary>
///     path: Game.Command.Unstuck
/// </summary>
public class UnstuckTextCHS
{
    public string Desc => "将玩家传送回默认场景";
    public string Usage => "用法: /unstuck [UID]";
    public string UnstuckSuccess => "已成功将该玩家传送回默认场景.";
    public string UidNotExist => "该UID不存在!";
    public string PlayerIsOnline => "该玩家目前在线上!";
}

/// <summary>
///     path: Game.Command.Setlevel
/// </summary>
public class SetlevelTextCHS
{
    public string Desc => "设定玩家等级";
    public string Usage => "用法: /setlevel [等级]";
    public string SetlevelSuccess => "等级设定成功!";
}

/// <summary>
///     path: Game.Command.Permission
/// </summary>
public class PermissionTextCHS
{
    public string Desc => "管理玩家权限";
    public string Usage =>
        "用法: /permission add [权限]\n" +
        "用法: /permission remove [权限]\n" +
        "用法: /permission clean [权限]";
    public string InvalidPerm => "权限 {0} 不存在!";
    public string Added => "已添加权限 {0} 到玩家 {1}!";
    public string Removed => "已移除玩家 {0} 的权限 {1}!";
    public string Cleaned => "已清除玩家 {0} 的所有权限!";
}

#endregion

#endregion
namespace NahidaImpact.Internationalization.Message;

#region Root

/// <summary>
///     English
/// </summary>
public class LanguageEN
{
    public GameTextEN Game { get; } = new();
    public ServerTextEN Server { get; } = new();
    public WordTextEN Word { get; } = new();
}

#endregion

#region Layer 1

/// <summary>
///     path: Game
/// </summary>
public class GameTextEN
{
    public CommandTextEN Command { get; } = new();
    public SceneInfoTextEN SceneInfo { get; } = new();
    public WorldInfoTextEN WorldInfo { get; } = new();
}

/// <summary>
///     path: Server
/// </summary>
public class ServerTextEN
{
    public WebTextEN Web { get; } = new();
    public ServerInfoTextEN ServerInfo { get; } = new();
    public ConnectionInfoTextEN ConnectionInfo { get; } = new();
}

/// <summary>
///     path: Word
/// </summary>
public class WordTextEN
{
    public string Config => "Config File";
    public string Language => "Language";
    public string GameData => "Game Data";
    public string CustomData => "Custom Data";
    public string Database => "Database";
    public string Command => "Command";
    public string Dispatch => "Global Dispatch";
    public string Game => "Game";
    public string Handbook => "Handbook";
    public string NotFound => "Not Found";
    public string Error => "Error";
    public string DatabaseAccount => "Database Account";
    public string Resource => "Resource";
}

#endregion

#region Layer 2

/// <summary>
///     path: Server.Web
/// </summary>
public class WebTextEN
{
    public string Maintain => "The server is undergoing maintenance, please try again later.";
    public string AccountNotFound => "Account not found";
    public string PasswordIncorrect => "Password incorrect";
    public string LoginFailed => "Account login failed";
    public string DecryptionFailed => "Account name decryption failed";
    public string CreateAccountFailed => "Failed to create account";
    public string TokenError => "Account token error";
    public string Relogin => "For account safety, please log in again";
    public string OK => "OK";
    public string CacheError => "Account cache error";
    public string ConnectionFailed => "Connection Failed!";
    public string ClientOutdated => "Server: {0} / Client: {1}\nPlease update.";
    public string ServerOutdated => "Server: {0} / Client: {1}\nPlease downgrade.";
}

/// <summary>
///     path: Server.ServerInfo
/// </summary>
public class ServerInfoTextEN
{
    public string Shutdown => "Shutting down...";
    public string CancelKeyPressed => "Cancel key pressed (Ctrl + C), server shutting down...";
    public string StartingServer => "Starting NahidaImpact";
    public string CurrentVersion => "Server supported versions: {0}";
    public string LoadingItem => "Loading {0}...";
    public string GeneratingItem => "Building {0}...";
    public string RegisterItem => "Registered {0} {1}(s).";
    public string FailedToLoadItem => "Failed to load {0}.";
    public string FailedToInitializeItem => "Failed to initialize {0}.";
    public string FailedToReadItem => "Failed to read {0}, file {1}";
    public string GeneratedItem => "Generated {0}.";

    public string LoadedItem => "Loaded {0}.";
    public string LoadedItems => "Loaded {0} {1}(s).";
    public string ServerRunning => "{0} server listening on {1}";
    public string ServerStarted => "Startup complete! Took {0}s. Type 'help' for command help";
    public string ConfigMissing => "{0} is missing. Please check: {1}, {2} may not be available.";
    public string SaveDatabase => "Database saved in {0}s";
    public string WaitForAllDone => "Please wait for all items to load before logging in.";
    public string UnhandledException => "An unhandled exception occurred: {0}";
    public string DirNotFound => "{0} directory not found: {1}";
    public string FailedToLoadData => "Failed to load {0} data.";
    public string FailedToLoadFile => "Failed to load {0}: {1}";

    public string LoadedScenePoints => "Loaded {0} scene points across {1} scenes";
    public string FileNotFound => "{0} file not found: {1}";
    public string HotfixNotAvailable => "Hotfix not available for {0}";
    public string HotfixCached => "Hotfix cached for {0}";
}

/// <summary>
///     path: Game.Command
/// </summary>
public class CommandTextEN
{
    public NoticeTextEN Notice { get; } = new();
    public HelpTextEN Help { get; } = new();
    public GiveTextEN Give { get; } = new();
    public GiveAllTextEN GiveAll { get; } = new();
    public TeleportTextEN Teleport { get; } = new();
    public AccountTextEN Account { get; } = new();
    public KickTextEN Kick { get; } = new();
    public SpawnTextEN Spawn { get; } = new();
    public StopTextEN Stop { get; } = new();
}

#endregion

#region Layer 3 - Command Texts

/// <summary>
///     path: Game.Command.Notice
/// </summary>
public class NoticeTextEN
{
    public string PlayerNotFound => "Player not found!";
    public string InvalidArguments => "Invalid arguments!";
    public string NoPermission => "You don't have permission!";
    public string CommandNotFound => "Command not found! Type '/help' for assistance";
    public string InternalError => "Internal error occurred while processing command!";
}

/// <summary>
///     path: Game.Command.Help
/// </summary>
public class HelpTextEN
{
    public string Desc => "Show help information";
    public string Usage => "Usage: /help\nUsage: /help [cmd]";
    public string Commands => "Commands: ";
    public string CommandPermission => "Permission: ";
    public string CommandAlias => "Aliases: ";
}

/// <summary>
///     path: Game.Command.Give
/// </summary>
public class GiveTextEN
{
    public string Desc => "Give items or avatars to a player";
    public string Usage => "Usage: /give <itemId|avatarId> [lv<level>] [x<amount>] [r<refinement>] [c<constellation>] [sl<skillLevel>]\n" +
                           "       /give <artifactId> [lv<level>] [x<amount>] [<mainPropId>] [<appendPropId[,times]>]...\n" +
                           "       /give <all|weapons|mats|furniture|avatars> [lv<level>] [x<amount>] [r<refinement>]";
    public string InvalidItemId => "Invalid item ID.";
    public string InvalidAmount => "Invalid amount.";
    public string GivenItem => "Gave {0}x item {1} to player {2}.";
    public string GivenWeapon => "Gave {0}x weapon {1} (level {2}, refinement {3}) to player {4}.";
    public string GivenRelic => "Gave {0}x relic {1} (level {2}) to player {3}.";
    public string GivenAvatar => "Gave avatar {0} (level {1}) to player {2}.";
    public string Failed => "Failed to give item {0}.";
}

/// <summary>
///     path: Game.Command.GiveAll
/// </summary>
public class GiveAllTextEN
{
    public string Desc => "Give all items of specified type";
    public string Usage => "Usage: /give <all|weapons|relics|mats|furniture|avatars> [lv<level>] [x<amount>] [r<refine>]";
    public string AvatarsGiven => "Gave {0} avatars (UID {1}).";
    public string WeaponsGiven => "Gave {0} weapons (UID {3}).";
    public string RelicsGiven => "Gave {0} relics (UID {3}).";
    public string MaterialsGiven => "Gave {0} material types x{1} (UID {3}).";
    public string FurnitureGiven => "Gave {0} furniture x{1} (UID {3}).";
    public string ItemsGiven => "Gave {0} items (UID {3}).";
    public string AllGiven => "All items given to UID {0}.";
}

/// <summary>
///     path: Game.Command.Teleport
/// </summary>
public class TeleportTextEN
{
    public string Desc => "Teleport player to coordinates";
    public string Usage => "Usage: /tp <x> <y> <z> [sceneId]";
    public string InvalidCoords => "Invalid coordinates.";
    public string Success => "Teleported {0} to ({1:F0}, {2:F0}, {3:F0}) scene {4}.";
    public string Failed => "Failed to teleport. Scene {0} may not exist.";
}

/// <summary>
///     path: Game.Command.Account
/// </summary>
public class AccountTextEN
{
    public string Desc => "Manage database accounts";
    public string Usage => "Usage: /account <create|delete> [username] [uid]";
    public string InvalidUid => "Invalid UID.";
    public string CreateSuccess => "Account '{0}' created with UID {1}.";
    public string DeleteSuccess => "Account with UID {0} deleted.";
}

/// <summary>
///     path: Game.Command.Kick
/// </summary>
public class KickTextEN
{
    public string Desc => "Kick a player from the server";
    public string Usage => "Usage: /kick @<uid>";
    public string PlayerKicked => "Kicked player {0}.";
    public string KickMessage => "You have been kicked by the server.";
}

/// <summary>
///     path: Game.Command.Spawn
/// </summary>
public class SpawnTextEN
{
    public string Desc => "Spawn entities";
    public string Usage => "Usage: /spawn <id> [x<amount>] [lv<level>] [state<state>] [ai<aiId>] [<x> <y> <z>]";
    public string InvalidEntityId => "Invalid entity ID.";
    public string NoScene => "Player is not in a scene.";
    public string Success => "Spawned {0} {1}(s) (ID: {2}, Lv: {3}) near player {4}.";
}

/// <summary>
///     path: Game.Command.Stop
/// </summary>
public class StopTextEN
{
    public string Desc => "Shutdown the server";
    public string Usage => "Usage: /stop";
    public string ShuttingDown => "Server shutting down...";
}

#endregion

#region Layer 3 - Connection / Scene / World log messages

/// <summary>
///     path: Server.ConnectionInfo
/// </summary>
public class ConnectionInfoTextEN
{
    public string NewConnection => "New connection from {0}.";
    public string ConnectionClosed => "Connection was closed";
    public string PacketTooLarge => "Packet too large";
    public string FailedToReceive => "Failed to receive packet";
    public string ParseError => "Packet parse error";
    public string BadDataPackage => "Bad Data Package Received: got 0x{0}, expect 0x4567";
    public string InvalidFooter => "Invalid packet footer received: got 0x{0}, expected 0x89AB";
    public string DummySend => "[Dummy] Send Dummy {0}";
    public string NoHandlerFound => "No handler found for packet {0} (0x{1})";
    public string RegionCacheError => "Error while initializing region cache!";
}

/// <summary>
///     path: Game.SceneInfo
/// </summary>
public class SceneInfoTextEN
{
    public string NoTeamManagerSpawn => "Player {0} has no TeamManager, cannot spawn avatar";
    public string NoAvatarEntity => "Player {0} has no current avatar entity to spawn";
    public string NoTeamManagerSetup => "Player {0} has no TeamManager, cannot setup avatars";
    public string NullEntity => "Attempted to add null entity to scene {0}";
    public string NoAvatarData => "No avatar data found! Check your ExcelBinOutput folder.";
}

/// <summary>
///     path: Game.WorldInfo
/// </summary>
public class WorldInfoTextEN
{
    public string NotifyPlayerInfo => "Notifying player {0} about new player {1}";
}

#endregion
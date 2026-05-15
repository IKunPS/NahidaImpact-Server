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
}

/// <summary>
///     path: Server
/// </summary>
public class ServerTextEN
{
    public WebTextEN Web { get; } = new();
    public ServerInfoTextEN ServerInfo { get; } = new();
}

/// <summary>
///     path: Word
/// </summary>
public class WordTextEN
{
    public string Config => "Config File";
    public string Language => "Language";
    public string Log => "Log";
    public string GameData => "Game Data";
    public string Cache => "Resource Cache";
    public string CustomData => "Custom Data";
    public string Database => "Database";
    public string Command => "Command";
    public string SdkServer => "Web Server";
    public string Handler => "Packet Handler";
    public string Dispatch => "Global Dispatch";
    public string Game => "Game";
    public string Handbook => "Handbook";
    public string NotFound => "Not Found";
    public string Error => "Error";
    public string DatabaseAccount => "Database Account";
    public string Tutorial => "Tutorial";
}

#endregion

#region Layer 2

/// <summary>
///     path: Server.Web
/// </summary>
public class WebTextEN
{
    public string Maintain => "The server is undergoing maintenance, please try again later.";
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
    public string InvalidVersion => "Unsupported game version {0}\nPlease update game to {1}";
    public string LoadingItem => "Loading {0}...";
    public string GeneratingItem => "Building {0}...";
    public string WaitingItem => "Waiting for process {0} to complete...";
    public string RegisterItem => "Registered {0} {1}(s).";
    public string FailedToLoadItem => "Failed to load {0}.";
    public string NewClientSecretKey => "Client Secret Key does not exist and a new Client Secret Key is being generated.";
    public string FailedToInitializeItem => "Failed to initialize {0}.";
    public string FailedToReadItem => "Failed to read {0}, file {1}";
    public string GeneratedItem => "Generated {0}.";
    public string LoadedItem => "Loaded {0}.";
    public string LoadedItems => "Loaded {0} {1}(s).";
    public string ServerRunning => "{0} server listening on {1}";
    public string ServerStarted => "Startup complete! Took {0}s. Type 'help' for command help";
    public string MissionEnabled => "Mission system enabled. This feature is still in development.";
    public string KeyStoreError => "The SSL certificate does not exist, SSL functionality has been disabled.";
    public string CacheLoadSkip => "Skipped cache loading.";
    public string ConfigMissing => "{0} is missing. Please check: {1}, {2} may not be available.";
    public string UnloadedItems => "Unloaded all {0}.";
    public string SaveDatabase => "Database saved in {0}s";
    public string WaitForAllDone => "Please wait for all items to load before logging in.";
    public string UnhandledException => "An unhandled exception occurred: {0}";
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
    public HealTextEN Heal { get; } = new();
    public ClearTextEN Clear { get; } = new();
    public AccountTextEN Account { get; } = new();
    public PermissionTextEN Permission { get; } = new();
    public KickTextEN Kick { get; } = new();
    public ListTextEN List { get; } = new();
    public SetPropTextEN SetProp { get; } = new();
    public UnlockAllTextEN UnlockAll { get; } = new();
    public SpawnTextEN Spawn { get; } = new();
    public KillAllTextEN KillAll { get; } = new();
    public MapAreaTextEN MapArea { get; } = new();
    public WeatherTextEN Weather { get; } = new();
    public PositionTextEN Position { get; } = new();
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
    public string PlayerNotInit => "Player not initialized.";
    public string InvalidArguments => "Invalid arguments!";
    public string NoPermission => "You don't have permission!";
    public string CommandNotFound => "Command not found! Type '/help' for assistance";
    public string TargetOffline => "Target {0}({1}) is offline!";
    public string TargetFound => "Target {0}({1}) found.";
    public string TargetNotFound => "Target {0} not found!";
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
    public string Desc => "Give items to a player";
    public string Usage => "Usage: /give <itemId> [amount] [level]";
    public string InvalidItemId => "Invalid item ID.";
    public string Success => "Gave {0}x item {1} (level {2}) to player {3}.";
    public string Failed => "Failed to give item {0}. Item type may not be supported yet.";
}

/// <summary>
///     path: Game.Command.GiveAll
/// </summary>
public class GiveAllTextEN
{
    public string Desc => "Give all items of specified type";
    public string Usage => "Usage: /giveall <avatars|weapons|relics|materials|essentials|all> [amount]";
    public string GiveAllItems => "Gave all {0} x{1}.";
    public string AvatarsGiven => "Gave {0} avatars to player {1}.";
    public string MaterialsGiven => "Gave {0} material types x{1} to player {2}.";
    public string WeaponsGiven => "Gave {0} weapons to player {1}.";
    public string RelicsGiven => "Gave {0} relics to player {1}.";
    public string EssentialsGiven => "Gave {0} essential items x{1} to player {2}.";
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
///     path: Game.Command.Heal
/// </summary>
public class HealTextEN
{
    public string Desc => "Heal all team members";
    public string Usage => "Usage: /heal";
    public string NoTeam => "Player has no team.";
    public string Success => "Healed all characters for player {0}.";
}

/// <summary>
///     path: Game.Command.Clear
/// </summary>
public class ClearTextEN
{
    public string Desc => "Clear player inventory";
    public string Usage => "Usage: /clear <weapons|materials|all>";
    public string ClearedAll => "Cleared all items for player {0}.";
    public string ClearedMaterials => "Cleared all materials for player {0}.";
    public string ClearedWeapons => "Cleared all weapons for player {0}.";
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
///     path: Game.Command.Permission
/// </summary>
public class PermissionTextEN
{
    public string Desc => "Manage player permissions";
    public string Usage => "Usage: /perm <add|remove|clear|list> [permission]";
    public string AccountNotFound => "Account not found.";
    public string InvalidPerm => "Invalid permission. Valid: {0}";
    public string Added => "Added permission '{0}' to UID {1}.";
    public string Removed => "Removed permission '{0}' from UID {1}.";
    public string Cleared => "Cleared all permissions for UID {0}.";
    public string List => "Permissions for UID {0}: {1}";
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
///     path: Game.Command.List
/// </summary>
public class ListTextEN
{
    public string Desc => "List online players";
    public string Usage => "Usage: /list";
    public string Header => "Online players ({0}):";
    public string Entry => "  UID: {0} | Name: {1} | Scene: {2} | Pos: ({3:F0}, {4:F0}, {5:F0})";
}

/// <summary>
///     path: Game.Command.SetProp
/// </summary>
public class SetPropTextEN
{
    public string Desc => "Set player properties";
    public string Usage => "Usage: /setprop <worldLevel|exp|name|signature> <value>";
    public string WorldLevel => "Set World Level to {0} for player {1}.";
    public string Exp => "Set EXP to {0} for player {1}.";
    public string Name => "Set name to '{0}' for player {1}.";
    public string Signature => "Set signature to '{0}' for player {1}.";
    public string Unknown => "Unknown property: {0}";
}

/// <summary>
///     path: Game.Command.UnlockAll
/// </summary>
public class UnlockAllTextEN
{
    public string Desc => "Unlock all scene points";
    public string Usage => "Usage: /unlockall";
    public string Success => "Unlocked {0} scene points for player {1} in scene {2}.";
}

/// <summary>
///     path: Game.Command.Spawn
/// </summary>
public class SpawnTextEN
{
    public string Desc => "Spawn monsters or gadgets";
    public string Usage => "Usage: /spawn <monster|gadget> <id> [amount] [level]";
    public string InvalidEntityId => "Invalid entity ID.";
    public string NoScene => "Player is not in a scene.";
    public string Success => "Spawned {0} {1}(s) (ID: {2}, Lv: {3}) near player {4}.";
}

/// <summary>
///     path: Game.Command.KillAll
/// </summary>
public class KillAllTextEN
{
    public string Desc => "Kill all monsters in scene";
    public string Usage => "Usage: /killall";
    public string Success => "Killed all monsters in current scene for player {0}.";
}

/// <summary>
/// <summary>
///     path: Game.Command.MapArea
/// </summary>
public class MapAreaTextEN
{
    public string Desc => "Manage map areas";
    public string Usage => "Usage: /maparea <give|remove|isopen> ...";
    public string Unknown => "Unknown subcommand: {0}";
    public string GiveUsage => "Usage: /maparea give <id|all> [isOpen]";
    public string GiveAll => "Gave all map areas to player {0} (isOpen={1})";
    public string GiveOne => "Gave map area {0} to player {1} (isOpen={2})";
    public string RemoveUsage => "Usage: /maparea remove <id>";
    public string RemoveSuccess => "Removed map area {0} from player {1}";
    public string SetOpenUsage => "Usage: /maparea isopen <id> <true|false>";
    public string SetOpenSuccess => "Set map area {0} isOpen={1} for player {2}";
    public string InvalidId => "Invalid map area ID: {0}";
    public string NotFound => "Map area {0} not found for player {1}";
    public string InvalidArgs => "Invalid arguments: id={0}, isOpen={1}";
}

/// <summary>
///     path: Game.Command.Weather
/// </summary>
public class WeatherTextEN
{
    public string Desc => "Change scene weather";
    public string Usage => "Usage: /weather <0-5>";
    public string Invalid => "Invalid weather type. Range: 0-5";
    public string Success => "Weather set to type {0} for player {1}.";
}

/// <summary>
///     path: Game.Command.Position
/// </summary>
public class PositionTextEN
{
    public string Desc => "Show current position";
    public string Usage => "Usage: /pos";
    public string PlayerInfo => "Player: {0} | Scene: {1}";
    public string PositionInfo => "Position: X={0:F1} Y={1:F1} Z={2:F1}";
    public string RotationInfo => "Rotation: X={0:F1} Y={1:F1} Z={2:F1}";
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
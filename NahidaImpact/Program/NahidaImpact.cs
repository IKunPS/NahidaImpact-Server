using NahidaImpact.Data;
using NahidaImpact.Database;
using NahidaImpact.NahidaImpact.Tool;
using NahidaImpact.GameServer.Command;
using NahidaImpact.GameServer.Server;
using NahidaImpact.Internationalization;
using NahidaImpact.KcpSharp;
using NahidaImpact.Util;
using System.Globalization;
using NahidaImpact.Util.Security;

namespace NahidaImpact.NahidaImpact.Program;

public class NahidaImpact
{
    public static readonly Logger Logger = new("NahidaImpact");
    public static readonly DatabaseHelper DatabaseHelper = new();
    public static readonly Listener Listener = new();
    public static readonly CommandManager CommandManager = new();

    public static async Task Main()
    {
        // Check if running as administrator
        if (!IsRunningAsAdministrator())
        {
            Console.WriteLine("This application requires administrator privileges to run.");
            Console.WriteLine("Please run the application as an administrator.");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            return;
        }

        var time = DateTime.Now;
        RegisterExitEvent();
        IConsole.InitConsole();
        LoaderManager.InitConfig();
        Crypto.LoadKeys();
        await LoaderManager.InitSdkServer();
        LoaderManager.InitPacket();

        LoaderManager.InitDatabase();
        if (!DatabaseHelper.LoadAllData)
        {
            DatabaseHelper.AllDataLoadedEvent.Wait();
            Logger.Info(I18NManager.Translate("Server.ServerInfo.LoadedItem", I18NManager.Translate("Word.Database")));
        }

        Logger.Warn(I18NManager.Translate("Server.ServerInfo.WaitForAllDone"));

        await LoaderManager.InitResource();
        ResourceManager.IsLoaded = true;

        HandbookGenerator.GenerateAll();
        LoaderManager.InitCommand();

        var elapsed = DateTime.Now - time;
        Logger.Info(I18NManager.Translate("Server.ServerInfo.ServerStarted",
            Math.Round(elapsed.TotalSeconds, 2).ToString(CultureInfo.InvariantCulture)));
    }

    private static bool IsRunningAsAdministrator()
    {
        if (OperatingSystem.IsWindows())
        {
            using var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(identity);
            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }

        // On Linux/macOS, check if running as root
        return Environment.UserName == "root";
    }

    # region Exit

    private static void RegisterExitEvent()
    {
        AppDomain.CurrentDomain.ProcessExit += (_, _) =>
        {
            Logger.Info(I18NManager.Translate("Server.ServerInfo.Shutdown"));
            ProcessExit();
        };
        AppDomain.CurrentDomain.UnhandledException += (obj, arg) =>
        {
            Logger.Error(I18NManager.Translate("Server.ServerInfo.UnhandledException", obj.GetType().Name),
                (Exception)arg.ExceptionObject);
            Logger.Info(I18NManager.Translate("Server.ServerInfo.Shutdown"));
            ProcessExit();
            Environment.Exit(1);
        };

        Console.CancelKeyPress += (_, eventArgs) =>
        {
            Logger.Info(I18NManager.Translate("Server.ServerInfo.CancelKeyPressed"));
            eventArgs.Cancel = true;
            Environment.Exit(0);
        };
    }

    private static void ProcessExit()
    {
        KcpListener.Connections.Values.ToList().ForEach(x => x.Stop(true));
        DatabaseHelper.SaveCts.Cancel();
        DatabaseHelper.SaveThread?.Join(TimeSpan.FromSeconds(5));
        DatabaseHelper.SaveDatabase();
    }

    # endregion
}
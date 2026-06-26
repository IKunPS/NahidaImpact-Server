using NahidaImpact.Configuration;
using NahidaImpact.Internationalization;
using Newtonsoft.Json;
using NahidaImpact.Util.Extensions;

namespace NahidaImpact.Util;

public static class ConfigManager
{
    public static readonly Logger Logger = new("ConfigManager");
    public static ConfigContainer Config { get; private set; } = new();
    private static readonly string ConfigFilePath = Config.Path.ConfigPath + "/Config.json";
    public static HotfixContainer Hotfix { get; private set; } = new();
    private static readonly string HotfixFilePath = Config.Path.ConfigPath + "/Hotfix.json";

    public static void LoadConfig()
    {
        LoadConfigData();
        LoadHotfixData();
    }

    private static void LoadConfigData()
    {
        var file = new FileInfo(ConfigFilePath);
        if (!file.Exists)
        {
            Config = new()
            {
                ServerOption =
                {
                    Language = Extensions.Extensions.CurrentLanguage
                }
            };

            Logger.Info("Current Language is " + Config.ServerOption.Language);
            SaveData(Config, ConfigFilePath);
            return; // Fresh config written, no need to re-read
        }

        using var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();
        Config = JsonConvert.DeserializeObject<ConfigContainer>(json)
                  ?? throw new InvalidOperationException("Config file is empty or corrupt.");

        SaveData(Config, ConfigFilePath);
    }

    private static void LoadHotfixData()
    {
        var file = new FileInfo(HotfixFilePath);

        var verList = Extensions.Extensions.SupportVersions;

        Logger.Info(I18NManager.Translate("Server.ServerInfo.CurrentVersion",
            verList.Aggregate((current, next) => $"{current}, {next}")));

        if (!file.Exists)
        {
            Hotfix = new HotfixContainer();
            SaveData(Hotfix, HotfixFilePath);
            return;
        }

        using var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();
        Hotfix = JsonConvert.DeserializeObject<HotfixContainer>(json)
                 ?? new HotfixContainer();

        SaveData(Hotfix, HotfixFilePath);
    }

    public static void SaveHotfix()
    {
        SaveData(Hotfix, HotfixFilePath);
    }

    private static void SaveData(object data, string path)
    {
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var json = JsonConvert.SerializeObject(data, Formatting.Indented);
        using var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
        using var writer = new StreamWriter(stream);
        writer.Write(json);
    }

    public static void InitDirectories()
    {
        foreach (var property in Config.Path.GetType().GetProperties())
        {
            var dir = property.GetValue(Config.Path)?.ToString();

            if (!string.IsNullOrEmpty(dir))
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
        }
    }
}
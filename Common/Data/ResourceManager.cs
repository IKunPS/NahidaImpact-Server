using System.Text.RegularExpressions;
using NahidaImpact.Data.Common;
using NahidaImpact.Data.Binout;
using NahidaImpact.Internationalization;
using NahidaImpact.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace NahidaImpact.Data;

public class ResourceManager
{
    public static Logger Logger { get; } = new("ResourceManager");
    public static bool IsLoaded { get; set; }

    public static void LoadGameData()
    {
        LoadAbilityModifiers();
        LoadAvatarConfigData();
        LoadAbilityGroups();
        LoadConfigLevelEntityData();
        LoadGlobalCombatConfig();
        LoadMonsterConfigData();
        LoadExcel();
        LoadScenePoints();
        LoadTrialAvatarCustomData();
    }

    public static void LoadExcel()
    {
        var classes = Assembly.GetExecutingAssembly().GetTypes(); // Get all classes in the assembly
        List<ExcelResource> resList = [];

        foreach (var cls in classes.Where(x => x.IsSubclassOf(typeof(ExcelResource))))
        {
            var res = LoadSingleExcelResource(cls);
            if (res != null) resList.AddRange(res);
        }

        foreach (var cls in resList) cls.AfterAllDone();
    }

    public static List<T>? LoadSingleExcel<T>(Type cls) where T : ExcelResource, new()
    {
        return LoadSingleExcelResource(cls) as List<T>;
    }

    public static List<ExcelResource>? LoadSingleExcelResource(Type cls)
    {
        var attribute = (ResourceEntity?)Attribute.GetCustomAttribute(cls, typeof(ResourceEntity));

        if (attribute == null) return null;
        var resource = (ExcelResource)Activator.CreateInstance(cls)!;
        var count = 0;
        List<ExcelResource> resList = [];
        foreach (var fileName in attribute.FileName)
            try
            {
                var path = ConfigManager.Config.Path.ResourcePath + "/ExcelBinOutput/" + fileName;
                var file = new FileInfo(path);
                if (!file.Exists)
                {
                    Logger.Error(I18NManager.Translate("Server.ServerInfo.FailedToReadItem", fileName,
                        I18NManager.Translate("Word.NotFound")));
                    continue;
                }

                var json = file.OpenText().ReadToEnd();
                using (var reader = new JsonTextReader(new StringReader(json)))
                {
                    reader.Read();
                    switch (reader.TokenType)
                    {
                        case JsonToken.StartArray:
                            {
                                // array
                                var jArray = JArray.Parse(json);
                                foreach (var item in jArray)
                                {
                                    var res = JsonConvert.DeserializeObject(item.ToString(), cls);
                                    resList.Add((ExcelResource)res!);
                                    ((ExcelResource?)res)?.Loaded();
                                    count++;
                                }

                                break;
                            }
                        case JsonToken.StartObject:
                            {
                                // dictionary
                                var jObject = JObject.Parse(json);
                                foreach (var (_, obj) in jObject)
                                {
                                    var instance = JsonConvert.DeserializeObject(obj!.ToString(), cls);

                                    if (((ExcelResource?)instance)?.GetId() == 0 || (ExcelResource?)instance == null)
                                    {
                                        // Deserialize as JObject to handle nested dictionaries
                                        var nestedObject = JsonConvert.DeserializeObject<JObject>(obj.ToString());

                                        foreach (var nestedItem in nestedObject ?? [])
                                        {
                                            var nestedInstance =
                                                JsonConvert.DeserializeObject(nestedItem.Value!.ToString(), cls);
                                            resList.Add((ExcelResource)nestedInstance!);
                                            ((ExcelResource?)nestedInstance)?.Loaded();
                                            count++;
                                        }
                                    }
                                    else
                                    {
                                        resList.Add((ExcelResource)instance);
                                        ((ExcelResource)instance).Loaded();
                                    }

                                    count++;
                                }

                                break;
                            }
                    }
                }

                resource.Finalized();
            }
            catch (Exception ex)
            {
                Logger.Error(
                    I18NManager.Translate("Server.ServerInfo.FailedToReadItem", fileName,
                        I18NManager.Translate("Word.Error")), ex);
            }

        Logger.Info(I18NManager.Translate("Server.ServerInfo.LoadedItems", count.ToString(), cls.Name));

        return resList;
    }

    public static T? LoadCustomFile<T>(string filetype, string filename)
    {
        var type = I18NManager.Translate("Word." + filetype);
        Logger.Info(I18NManager.Translate("Server.ServerInfo.LoadingItem", type));
        FileInfo file = new(ConfigManager.Config.Path.DataPath + $"/{filename}.json");
        T? customFile = default;
        if (!file.Exists)
        {
            Logger.Warn(I18NManager.Translate("Server.ServerInfo.ConfigMissing", type,
                $"{ConfigManager.Config.Path.DataPath}/{filename}.json", type));
            return customFile;
        }

        try
        {
            using var reader = file.OpenRead();
            using StreamReader reader2 = new(reader);
            var text = reader2.ReadToEnd();
            var json = JsonConvert.DeserializeObject<T>(text);
            customFile = json;
        }
        catch (Exception ex)
        {
            Logger.Error("Error in reading " + file.Name, ex);
        }

        switch (customFile)
        {
            case Dictionary<int, int> d:
                Logger.Info(I18NManager.Translate("Server.ServerInfo.LoadedItems", d.Count.ToString(), type));
                break;
            case Dictionary<int, List<int>> di:
                Logger.Info(I18NManager.Translate("Server.ServerInfo.LoadedItems", di.Count.ToString(), type));
                break;
            default:
                Logger.Info(I18NManager.Translate("Server.ServerInfo.LoadedItem", filetype));
                break;
        }

        return customFile;
    }

    private static void LoadScenePoints()
    {
        var pattern = new Regex(@"scene(\d+)_point\.json");
        var pointDir = Path.Combine(ConfigManager.Config.Path.ResourcePath, "BinOutput", "Scene", "Point");

        if (!Directory.Exists(pointDir))
        {
            Logger.Warn($"Scene point directory not found: {pointDir}. Teleport waypoints will not work.");
            return;
        }

        try
        {
            foreach (var filePath in Directory.EnumerateFiles(pointDir, "scene*_point.json"))
            {
                var fileName = Path.GetFileName(filePath);
                var match = pattern.Match(fileName);
                if (!match.Success) continue;

                int sceneId = int.Parse(match.Groups[1].Value);

                var json = File.ReadAllText(filePath);
                var root = JObject.Parse(json);
                var pointsToken = root["points"];
                if (pointsToken == null) continue;
                var pointsDict = pointsToken.ToObject<Dictionary<int, PointData>>();
                if (pointsDict == null) continue;

                var scenePoints = new List<int>();
                foreach (var (pointId, pointData) in pointsDict)
                {
                    pointData.Id = pointId;
                    var entry = new ConfigScenePointEntry(sceneId, pointData);

                    scenePoints.Add(pointId);
                    GameData.ScenePointIdList.Add(pointId);
                    GameData.ScenePointEntry[(sceneId << 16) + pointId] = entry;
                }

                GameData.ScenePointsPerScene[sceneId] = scenePoints;
            }

            Logger.Info($"Loaded {GameData.ScenePointEntry.Count} scene points across {GameData.ScenePointsPerScene.Count} scenes.");
        }
        catch (Exception ex)
        {
            Logger.Error("Failed to load scene points.", ex);
        }
    }
    
    private static void LoadConfigLevelEntityData()
    {
        var levelEntityDir = Path.Combine(ConfigManager.Config.Path.ResourcePath, "BinOutput", "LevelEntity");
        if (!Directory.Exists(levelEntityDir))
        {
            Logger.Warn($"LevelEntity directory not found: {levelEntityDir}");
            return;
        }

        try
        {
            int count = 0;
            foreach (var filePath in Directory.EnumerateFiles(levelEntityDir, "*.json"))
            {
                try
                {
                    var json = File.ReadAllText(filePath);
                    var dict = JsonConvert.DeserializeObject<Dictionary<string, ConfigLevelEntity>>(json);
                    if (dict == null) continue;

                    foreach (var kv in dict)
                    {
                        GameData.ConfigLevelEntityDataMap[kv.Key] = kv.Value;
                        count++;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"Failed to load LevelEntity file: {Path.GetFileName(filePath)}", ex);
                }
            }

            Logger.Info($"Loaded {count} ConfigLevelEntity entries from {levelEntityDir}");
        }
        catch (Exception ex)
        {
            Logger.Error("Failed to load ConfigLevelEntity data.", ex);
        }
    }
    
    private static void LoadGlobalCombatConfig()
    {
        var filePath = Path.Combine(ConfigManager.Config.Path.ResourcePath, "BinOutput", "Common", "ConfigGlobalCombat.json");
        if (!File.Exists(filePath))
        {
            Logger.Warn($"ConfigGlobalCombat file not found: {filePath}");
            return;
        }

        try
        {
            var json = File.ReadAllText(filePath);
            GameData.ConfigGlobalCombat = JsonConvert.DeserializeObject<ConfigGlobalCombat>(json);
            Logger.Info("Loaded ConfigGlobalCombat");
        }
        catch (Exception ex)
        {
            Logger.Error("Failed to load ConfigGlobalCombat.", ex);
        }
    }
    
    private static readonly JsonSerializerSettings AbilitySerializerSettings = new()
    {
        Error = (_, args) =>
        {
            // Some numeric fields in ability JSON contain string formula names.
            // Ignore these conversion errors (mirrors Java's tolerant parsing).
            args.ErrorContext.Handled = true;
        }
    };

    private static void LoadAbilityModifiers()
    {
        var abilityDir = Path.Combine(ConfigManager.Config.Path.ResourcePath, "BinOutput", "Ability", "Temp");
        if (!Directory.Exists(abilityDir))
        {
            Logger.Warn($"Ability directory not found: {abilityDir}");
            return;
        }

        try
        {
            var serializer = JsonSerializer.Create(AbilitySerializerSettings);
            int count = 0;
            var allFiles = Directory.EnumerateFiles(abilityDir, "*.json", SearchOption.AllDirectories);
            foreach (var filePath in allFiles)
            {
                try
                {
                    var json = File.ReadAllText(filePath);
                    var array = JArray.Parse(json);
                    foreach (var item in array)
                    {
                        var defaultToken = item["Default"];
                        if (defaultToken == null) continue;

                        var abilityData = defaultToken.ToObject<Ability.AbilityData>(serializer);
                        if (abilityData == null || string.IsNullOrEmpty(abilityData.AbilityName)) continue;

                        abilityData.Initialize();
                        GameData.AbilityData[abilityData.AbilityName] = abilityData;
                        count++;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"Failed to load ability file: {Path.GetFileName(filePath)}", ex);
                }
            }

            Logger.Info($"Loaded {count} ability modifiers from {abilityDir}");
        }
        catch (Exception ex)
        {
            Logger.Error("Failed to load ability modifiers.", ex);
        }
    }
    
    private static void LoadAvatarConfigData()
    {
        var avatarDir = Path.Combine(ConfigManager.Config.Path.ResourcePath, "BinOutput", "Avatar");
        if (!Directory.Exists(avatarDir))
        {
            Logger.Warn($"Avatar config directory not found: {avatarDir}");
            return;
        }

        var configPattern = new Regex(@"^ConfigAvatar_(.+?)\.json$");

        try
        {
            int count = 0;
            foreach (var filePath in Directory.EnumerateFiles(avatarDir, "*.json"))
            {
                try
                {
                    var fileName = Path.GetFileName(filePath);
                    var match = configPattern.Match(fileName);
                    var key = match.Success ? match.Groups[1].Value : Path.GetFileNameWithoutExtension(fileName);

                    var json = File.ReadAllText(filePath);
                    var config = JsonConvert.DeserializeObject<ConfigEntityAvatar>(json);
                    if (config != null)
                    {
                        GameData.AvatarConfigData[key] = config;
                        count++;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"Failed to load avatar config: {Path.GetFileName(filePath)}", ex);
                }
            }

            Logger.Info($"Loaded {count} avatar configs from {avatarDir}");
        }
        catch (Exception ex)
        {
            Logger.Error("Failed to load avatar configs.", ex);
        }
    }
    
    private static void LoadAbilityGroups()
    {
        var abilityGroupDir = Path.Combine(ConfigManager.Config.Path.ResourcePath, "BinOutput", "AbilityGroup");
        if (!Directory.Exists(abilityGroupDir))
        {
            Logger.Warn($"AbilityGroup directory not found: {abilityGroupDir}");
            return;
        }

        try
        {
            int count = 0;
            foreach (var filePath in Directory.EnumerateFiles(abilityGroupDir, "*.json"))
            {
                try
                {
                    var json = File.ReadAllText(filePath);
                    var dict = JsonConvert.DeserializeObject<Dictionary<string, ConfigEntityBase>>(json);
                    if (dict != null)
                    {
                        foreach (var kv in dict)
                        {
                            GameData.PlayerAbilities[kv.Key] = kv.Value;
                            count++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"Failed to load AbilityGroup file: {Path.GetFileName(filePath)}", ex);
                }
            }

            Logger.Info($"Loaded {count} player ability groups from {abilityGroupDir}");
        }
        catch (Exception ex)
        {
            Logger.Error("Failed to load AbilityGroup data.", ex);
        }
    }

    private static void LoadTrialAvatarCustomData()
    {
        var filePath = Path.Combine(ConfigManager.Config.Path.ResourcePath,
            "CustomResources", "TrialAvatarExcels", "TrialAvatarData.json");
        if (!File.Exists(filePath))
        {
            Logger.Warn($"TrialAvatarCustomData file not found: {filePath}");
            return;
        }

        try
        {
            var json = File.ReadAllText(filePath);
            var list = JsonConvert.DeserializeObject<List<TrialAvatarCustomData>>(json);
            if (list == null) return;

            foreach (var entry in list)
            {
                // Filter blank strings, mirroring Java onLoad()
                entry.TrialAvatarParamList = entry.TrialAvatarParamList
                    .Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

                GameData.TrialAvatarCustomDataMap[entry.TrialAvatarId] = entry;
            }

            Logger.Info($"Loaded {list.Count} trial avatar custom data entries");
        }
        catch (Exception ex)
        {
            Logger.Error("Failed to load TrialAvatarCustomData.", ex);
        }
    }
    
    private static void LoadMonsterConfigData()
    {
        var monsterDir = Path.Combine(ConfigManager.Config.Path.ResourcePath, "BinOutput", "Monster");
        if (!Directory.Exists(monsterDir))
        {
            Logger.Warn($"Monster config directory not found: {monsterDir}");
            return;
        }

        try
        {
            int count = 0;
            foreach (var filePath in Directory.EnumerateFiles(monsterDir, "*.json"))
            {
                try
                {
                    var name = Path.GetFileNameWithoutExtension(filePath);
                    var json = File.ReadAllText(filePath);
                    var config = JsonConvert.DeserializeObject<ConfigEntityMonster>(json);
                    if (config != null)
                    {
                        GameData.MonsterConfigData[name] = config;
                        count++;
                    }
                }
                catch { }
            }
            Logger.Info($"Loaded {count} monster configs from {monsterDir}");
        }
        catch (Exception ex)
        {
            Logger.Error("Failed to load monster config data.", ex);
        }
    }
}
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

    private static readonly JsonSerializerSettings ExcelSettings = new()
    {
        MissingMemberHandling = MissingMemberHandling.Ignore
    };

    public static void LoadGameData()
    {
        LoadAbilityModifiers();
        LoadAvatarConfigData();
        LoadAbilityGroups();
        LoadConfigLevelEntityData();
        LoadGlobalCombatConfig();
        LoadMonsterConfigData();
        LoadScenePoints();
        LoadExcel();
    }

    public static void LoadExcel()
    {
        var classes = Assembly.GetExecutingAssembly().GetTypes();
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

                string json;
                using (var textReader = file.OpenText())
                    json = textReader.ReadToEnd();
                using (var reader = new JsonTextReader(new StringReader(json)))
                {
                    reader.Read();
                    switch (reader.TokenType)
                    {
                        case JsonToken.StartArray:
                            {
                                var jArray = JArray.Parse(json);
                                foreach (var item in jArray)
                                {
                                    var res = JsonConvert.DeserializeObject(item.ToString(), cls, ExcelSettings);
                                    if (res != null)
                                    {
                                        resList.Add((ExcelResource)res);
                                        ((ExcelResource)res).Loaded();
                                        count++;
                                    }
                                }

                                break;
                            }
                        case JsonToken.StartObject:
                            {
                                var jObject = JObject.Parse(json);
                                foreach (var (_, obj) in jObject)
                                {
                                    var instance = JsonConvert.DeserializeObject(obj!.ToString(), cls, ExcelSettings);

                                    if (instance == null || ((ExcelResource)instance).GetId() == 0)
                                    {
                                        var nestedObject = JsonConvert.DeserializeObject<JObject>(obj.ToString());
                                        foreach (var nestedItem in nestedObject ?? [])
                                        {
                                            var nestedInstance =
                                                JsonConvert.DeserializeObject(nestedItem.Value!.ToString(), cls, ExcelSettings);
                                            if (nestedInstance != null)
                                            {
                                                resList.Add((ExcelResource)nestedInstance);
                                                ((ExcelResource)nestedInstance).Loaded();
                                                count++;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        resList.Add((ExcelResource)instance);
                                        ((ExcelResource)instance).Loaded();
                                        count++;
                                    }
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
            var json = JsonConvert.DeserializeObject<T>(text, ExcelSettings);
            customFile = json;
        }
        catch (Exception ex)
        {
            Logger.Error(I18NManager.Translate("Server.ServerInfo.FailedToReadItem", file.Name,
                I18NManager.Translate("Word.Error")), ex);
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
                Logger.Info(I18NManager.Translate("Server.ServerInfo.LoadedItems", "1", type));
                break;
        }

        return customFile;
    }

    private static void LoadScenePoints()
    {
        var word = I18NManager.Translate("Word.Resource");
        var pattern = new Regex(@"scene(\d+)_point\.json");
        var pointDir = Path.Combine(ConfigManager.Config.Path.ResourcePath, "BinOutput", "Scene", "Point");

        if (!Directory.Exists(pointDir))
        {
            Logger.Warn(I18NManager.Translate("Server.ServerInfo.DirNotFound", word, pointDir));
            return;
        }

        try
        {
            foreach (var filePath in Directory.EnumerateFiles(pointDir, "scene*_point.json"))
            {
                var fileName = Path.GetFileName(filePath);
                var match = pattern.Match(fileName);
                if (!match.Success) continue;

                if (!int.TryParse(match.Groups[1].Value, out var sceneId)) continue;

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

            Logger.Info(I18NManager.Translate("Server.ServerInfo.LoadedItems",
                GameData.ScenePointEntry.Count.ToString(), "scene points"));
        }
        catch (Exception ex)
        {
            Logger.Error(I18NManager.Translate("Server.ServerInfo.FailedToLoadData", "scene points"), ex);
        }
    }

    private static void LoadConfigLevelEntityData()
    {
        var word = I18NManager.Translate("Word.Resource");
        var levelEntityDir = Path.Combine(ConfigManager.Config.Path.ResourcePath, "BinOutput", "LevelEntity");
        if (!Directory.Exists(levelEntityDir))
        {
            Logger.Warn(I18NManager.Translate("Server.ServerInfo.DirNotFound", word, levelEntityDir));
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
                    var name = I18NManager.Translate("Word.Resource");
                    Logger.Error(I18NManager.Translate("Server.ServerInfo.FailedToLoadFile",
                        name, Path.GetFileName(filePath)), ex);
                }
            }

            Logger.Info(I18NManager.Translate("Server.ServerInfo.LoadedItems",
                count.ToString(), "ConfigLevelEntity"));
        }
        catch (Exception ex)
        {
            Logger.Error(I18NManager.Translate("Server.ServerInfo.FailedToLoadData", "ConfigLevelEntity"), ex);
        }
    }

    private static void LoadGlobalCombatConfig()
    {
        var word = I18NManager.Translate("Word.Resource");
        var filePath = Path.Combine(ConfigManager.Config.Path.ResourcePath, "BinOutput", "Common", "ConfigGlobalCombat.json");
        if (!File.Exists(filePath))
        {
            Logger.Warn(I18NManager.Translate("Server.ServerInfo.FileNotFound", word, filePath));
            return;
        }

        try
        {
            var json = File.ReadAllText(filePath);
            GameData.ConfigGlobalCombat = JsonConvert.DeserializeObject<ConfigGlobalCombat>(json);
            Logger.Info(I18NManager.Translate("Server.ServerInfo.LoadedItems", "1", "ConfigGlobalCombat"));
        }
        catch (Exception ex)
        {
            Logger.Error(I18NManager.Translate("Server.ServerInfo.FailedToLoadData", "ConfigGlobalCombat"), ex);
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
        var word = I18NManager.Translate("Word.Resource");
        var abilityDir = Path.Combine(ConfigManager.Config.Path.ResourcePath, "BinOutput", "Ability", "Temp");
        if (!Directory.Exists(abilityDir))
        {
            Logger.Warn(I18NManager.Translate("Server.ServerInfo.DirNotFound", word, abilityDir));
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
                    Logger.Error(I18NManager.Translate("Server.ServerInfo.FailedToLoadFile",
                        word, Path.GetFileName(filePath)), ex);
                }
            }

            Logger.Info(I18NManager.Translate("Server.ServerInfo.LoadedItems",
                count.ToString(), "ability modifier"));
        }
        catch (Exception ex)
        {
            Logger.Error(I18NManager.Translate("Server.ServerInfo.FailedToLoadData", "ability modifiers"), ex);
        }
    }

    private static void LoadAvatarConfigData()
    {
        var word = I18NManager.Translate("Word.Resource");
        var avatarDir = Path.Combine(ConfigManager.Config.Path.ResourcePath, "BinOutput", "Avatar");
        if (!Directory.Exists(avatarDir))
        {
            Logger.Warn(I18NManager.Translate("Server.ServerInfo.DirNotFound", word, avatarDir));
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
                    Logger.Error(I18NManager.Translate("Server.ServerInfo.FailedToLoadFile",
                        word, Path.GetFileName(filePath)), ex);
                }
            }

            Logger.Info(I18NManager.Translate("Server.ServerInfo.LoadedItems",
                count.ToString(), "avatar config"));
        }
        catch (Exception ex)
        {
            Logger.Error(I18NManager.Translate("Server.ServerInfo.FailedToLoadData", "avatar configs"), ex);
        }
    }

    private static void LoadAbilityGroups()
    {
        var word = I18NManager.Translate("Word.Resource");
        var abilityGroupDir = Path.Combine(ConfigManager.Config.Path.ResourcePath, "BinOutput", "AbilityGroup");
        if (!Directory.Exists(abilityGroupDir))
        {
            Logger.Warn(I18NManager.Translate("Server.ServerInfo.DirNotFound", word, abilityGroupDir));
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
                    var trimmed = json.TrimStart();
                    if (trimmed.StartsWith("["))
                    {
                        var jArray = JArray.Parse(json);
                        foreach (var item in jArray)
                        {
                            if (item is JObject jObj)
                            {
                                var properties = jObj.Properties().ToList();
                                if (properties.Count == 1 && properties[0].Value is JObject)
                                {
                                    var key = properties[0].Name;
                                    var value = properties[0].Value.ToObject<ConfigEntityBase>();
                                    if (value != null)
                                    {
                                        GameData.PlayerAbilities[key] = value;
                                        count++;
                                    }
                                }
                                else
                                {
                                    var value = item.ToObject<ConfigEntityBase>();
                                    if (value != null)
                                    {
                                        var key = Path.GetFileNameWithoutExtension(filePath);
                                        GameData.PlayerAbilities[key] = value;
                                        count++;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
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
                }
                catch (Exception ex)
                {
                    Logger.Error(I18NManager.Translate("Server.ServerInfo.FailedToLoadFile",
                        word, Path.GetFileName(filePath)), ex);
                }
            }

            Logger.Info(I18NManager.Translate("Server.ServerInfo.LoadedItems",
                count.ToString(), "player ability group"));
        }
        catch (Exception ex)
        {
            Logger.Error(I18NManager.Translate("Server.ServerInfo.FailedToLoadData", "AbilityGroup"), ex);
        }
    }

    private static void LoadMonsterConfigData()
    {
        var word = I18NManager.Translate("Word.Resource");
        var monsterDir = Path.Combine(ConfigManager.Config.Path.ResourcePath, "BinOutput", "Monster");
        if (!Directory.Exists(monsterDir))
        {
            Logger.Warn(I18NManager.Translate("Server.ServerInfo.DirNotFound", word, monsterDir));
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
                catch (Exception ex)
                {
                    Logger.Debug(I18NManager.Translate("Server.ServerInfo.FailedToLoadFile",
                        word, Path.GetFileName(filePath) + " - " + ex.Message));
                }
            }
            Logger.Info(I18NManager.Translate("Server.ServerInfo.LoadedItems",
                count.ToString(), "monster config"));
        }
        catch (Exception ex)
        {
            Logger.Error(I18NManager.Translate("Server.ServerInfo.FailedToLoadData", "monster config"), ex);
        }
    }
}

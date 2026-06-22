using NahidaImpact.Data.Ability;
using NahidaImpact.Data.Binout;
using NahidaImpact.Data.Common;
using NahidaImpact.Data.Excel;

namespace NahidaImpact.Data;

public static class GameData
{
    public static Dictionary<int, AvatarDataExcel> AvatarData { get; private set; } = [];
    public static Dictionary<int, AvatarSkillDataExcel> AvatarSkillData { get; private set; } = [];
    public static Dictionary<int, AvatarSkillDepotDataExcel> AvatarSkillDepotData { get; private set; } = [];
    public static Dictionary<int, ProudSkillDataExcel> ProudSkillData { get; private set; } = [];
    public static Dictionary<int, AvatarTalentDataExcel> AvatarTalentData { get; private set; } = [];
    public static Dictionary<int, OpenStateDataExcel> OpenStateData { get; private set; } = [];
    public static Dictionary<string, AbilityData> AbilityData { get; private set; } = [];
    public static Dictionary<int, SceneDataExcel> SceneData { get; private set; } = [];

    public static Dictionary<int, MonsterDataExcel> MonsterData { get; private set; } = [];

    // Scene Points
    public static Dictionary<int, ConfigScenePointEntry> ScenePointEntry { get; private set; } = [];
    public static List<int> ScenePointIdList { get; private set; } = [];
    public static Dictionary<int, List<int>> ScenePointsPerScene { get; private set; } = [];

    // Map Area
    public static Dictionary<int, MapAreaDataExcel> MapAreaData { get; private set; } = [];

    // Map Layer
    public static Dictionary<int, MapLayerDataExcel> MapLayerData { get; private set; } = [];
    public static Dictionary<int, MapLayerFloorDataExcel> MapLayerFloorData { get; private set; } = [];
    public static Dictionary<int, MapLayerGroupDataExcel> MapLayerGroupData { get; private set; } = [];

    // Scene Tags
    public static Dictionary<int, ItemDataExcel> ItemData { get; private set; } = [];
    public static Dictionary<string, ConfigEntityMonster> MonsterConfigData { get; private set; } = [];
    public static Dictionary<int, SceneTagDataExcel> SceneTagData { get; private set; } = [];

    // Level entity config
    public static Dictionary<string, ConfigLevelEntity> ConfigLevelEntityDataMap { get; private set; } = [];

    // Global combat
    public static ConfigGlobalCombat? ConfigGlobalCombat { get; set; }

    // Avatar config
    public static Dictionary<string, ConfigEntityAvatar> AvatarConfigData { get; private set; } = [];

    // Skill depot ability groups
    public static Dictionary<string, ConfigEntityBase> PlayerAbilities { get; private set; } = [];

    // Trial avatar
    public static Dictionary<int, TrialAvatarDataExcel> TrialAvatarDataMap { get; private set; } = [];

    // Weapon
    public static Dictionary<int, WeaponPromoteDataExcel> WeaponPromoteData { get; private set; } = [];
    public static Dictionary<int, WeaponLevelDataExcel> WeaponLevelData { get; private set; } = [];

    // Avatar level
    public static Dictionary<int, AvatarLevelDataExcel> AvatarLevelData { get; private set; } = [];
    public static Dictionary<int, AvatarCurveDataExcel> AvatarCurveData { get; private set; } = [];
    public static Dictionary<int, AvatarPromoteDataExcel> AvatarPromoteData { get; private set; } = [];

    public static AbilityData? GetAbilityData(string abilityName)
    {
        AbilityData.TryGetValue(abilityName, out var data);
        return data;
    }

    public static string? GetAbilityNameByHash(uint hash)
    {
        foreach (var (name, _) in AbilityData)
        {
            if (Util.Utils.AbilityHash(name) == hash)
                return name;
        }
        return null;
    }


    public static string? GetAbilityNameByHash(int hash)
    {
        return GetAbilityNameByHash((uint)hash);
    }

    public static ConfigScenePointEntry? GetScenePointEntryById(int sceneId, int pointId)
    {
        ScenePointEntry.TryGetValue((sceneId << 16) + pointId, out var entry);
        return entry;
    }
}
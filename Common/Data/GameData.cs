using NahidaImpact.Data.Ability;
using NahidaImpact.Data.Excel;

namespace NahidaImpact.Data;

public static class GameData
{
    public static Dictionary<int, AvatarDataExcel> AvatarData { get; private set; } = [];
    public static Dictionary<int, OpenStateDataExcel> OpenStateData { get; private set; } = [];
    public static Dictionary<string, AbilityData> AbilityData { get; private set; } = [];
    public static Dictionary<int, SceneDataExcel> SceneData { get; private set; } = [];
}
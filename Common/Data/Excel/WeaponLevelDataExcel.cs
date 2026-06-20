using System.Text.Json.Serialization;

namespace NahidaImpact.Data.Excel;

[ResourceEntity("WeaponLevelExcelConfigData.json")]
public class WeaponLevelDataExcel : ExcelResource
{
    [JsonPropertyName("level")]
    public int Level { get; set; }

    [JsonPropertyName("requiredExps")]
    public List<int> RequiredExps { get; set; } = [];

    public override uint GetId() => (uint)Level;

    public override void Loaded()
    {
        GameData.WeaponLevelData[Level] = this;
    }
}
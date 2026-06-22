using Newtonsoft.Json;

namespace NahidaImpact.Data.Excel;

[ResourceEntity("WeaponLevelExcelConfigData.json")]
public class WeaponLevelDataExcel : ExcelResource
{
    [JsonProperty("level")]
    public int Level { get; set; }

    [JsonProperty("requiredExps")]
    public List<int> RequiredExps { get; set; } = [];

    public override uint GetId() => (uint)Level;

    public override void Loaded()
    {
        GameData.WeaponLevelData[Level] = this;
    }
}
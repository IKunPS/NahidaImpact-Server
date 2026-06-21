using Newtonsoft.Json;

namespace NahidaImpact.Data.Excel;

[ResourceEntity("AvatarLevelExcelConfigData.json")]
public class AvatarLevelDataExcel : ExcelResource
{
    [JsonProperty("level")] public int Level { get; set; }
    [JsonProperty("exp")] public int Exp { get; set; }

    public override uint GetId() => (uint)Level;

    public override void Loaded()
    {
        GameData.AvatarLevelData[Level] = this;
    }
}

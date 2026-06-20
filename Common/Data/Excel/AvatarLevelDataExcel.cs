using System.Text.Json.Serialization;

namespace NahidaImpact.Data.Excel;

[ResourceEntity("AvatarLevelExcelConfigData.json")]
public class AvatarLevelDataExcel : ExcelResource
{
    [JsonPropertyName("level")] public int Level { get; set; }
    [JsonPropertyName("exp")] public int Exp { get; set; }

    public override uint GetId() => (uint)Level;

    public override void Loaded()
    {
        GameData.AvatarLevelData[Level] = this;
    }
}

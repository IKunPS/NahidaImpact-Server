using System.Text.Json.Serialization;
using NahidaImpact.Data.Common;

namespace NahidaImpact.Data.Excel;

[ResourceEntity("AvatarCurveExcelConfigData.json")]
public class AvatarCurveDataExcel : ExcelResource
{
    [JsonPropertyName("level")] public int Level { get; set; }
    [JsonPropertyName("curveInfos")] public List<CurveInfo> CurveInfos { get; set; } = [];

    public override uint GetId() => (uint)Level;

    public override void Loaded()
    {
        GameData.AvatarCurveData[Level] = this;
    }
}

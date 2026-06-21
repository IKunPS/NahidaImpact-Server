using Newtonsoft.Json;
using NahidaImpact.Data.Common;

namespace NahidaImpact.Data.Excel;

[ResourceEntity("AvatarCurveExcelConfigData.json")]
public class AvatarCurveDataExcel : ExcelResource
{
    [JsonProperty("level")] public int Level { get; set; }
    [JsonProperty("curveInfos")] public List<CurveInfo> CurveInfos { get; set; } = [];

    public override uint GetId() => (uint)Level;

    public override void Loaded()
    {
        GameData.AvatarCurveData[Level] = this;
    }
}

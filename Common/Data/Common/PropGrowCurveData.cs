using Newtonsoft.Json;

namespace NahidaImpact.Data.Common;

public class PropGrowCurveData
{
    [JsonProperty("type")] public string Type { get; set; } = "";
    [JsonProperty("growCurve")] public string GrowCurve { get; set; } = "";
}

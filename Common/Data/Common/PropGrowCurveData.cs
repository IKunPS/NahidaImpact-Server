using System.Text.Json.Serialization;

namespace NahidaImpact.Data.Common;

public class PropGrowCurveData
{
    [JsonPropertyName("type")] public string Type { get; set; } = "";
    [JsonPropertyName("growCurve")] public string GrowCurve { get; set; } = "";
}

using System.Text.Json.Serialization;

namespace NahidaImpact.Data.Common;

public class CurveInfo
{
    [JsonPropertyName("type")] public string Type { get; set; } = "";
    [JsonPropertyName("arith")] public string Arith { get; set; } = "ARITH_MULTI";
    [JsonPropertyName("value")] public float Value { get; set; }
}

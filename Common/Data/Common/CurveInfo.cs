using Newtonsoft.Json;

namespace NahidaImpact.Data.Common;

public class CurveInfo
{
    [JsonProperty("type")] public string Type { get; set; } = "";
    [JsonProperty("arith")] public string Arith { get; set; } = "ARITH_MULTI";
    [JsonProperty("value")] public float Value { get; set; }
}

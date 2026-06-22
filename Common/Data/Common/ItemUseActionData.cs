using Newtonsoft.Json;

namespace NahidaImpact.Data.Common;

public class ItemUseActionData
{
    [JsonProperty("useOp")] public string UseOp { get; set; } = "";
    [JsonProperty("useParam")] public List<string> UseParam { get; set; } = [];

    /// <summary>First useParam parsed as float.</summary>
    [JsonIgnore]
    public float Param
    {
        get
        {
            if (UseParam.Count > 0 && float.TryParse(UseParam[0], out var v))
                return v;
            return 0f;
        }
    }
}

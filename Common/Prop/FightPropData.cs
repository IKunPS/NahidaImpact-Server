using Newtonsoft.Json;

namespace NahidaImpact.Prop;

public class FightPropData
{
    [JsonProperty("propType")]
    public string PropType { get; set; } = "";

    [JsonProperty("value")]
    public float Value { get; set; }

    public void OnLoad()
    {
        // no-op: PropType is stored as string
    }
}
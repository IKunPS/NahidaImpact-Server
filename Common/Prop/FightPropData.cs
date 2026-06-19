using System.Text.Json.Serialization;

namespace NahidaImpact.Prop;

public class FightPropData
{
    [JsonPropertyName("propType")]
    public string PropType { get; set; } = "";

    [JsonPropertyName("value")]
    public float Value { get; set; }

    public void OnLoad()
    {
        // no-op: PropType is stored as string
    }
}
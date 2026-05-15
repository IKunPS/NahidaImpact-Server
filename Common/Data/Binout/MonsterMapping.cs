using Newtonsoft.Json;

namespace NahidaImpact.Data.Binout;

public class MonsterMapping
{
    [JsonProperty("monsterId")]
    public int MonsterId { get; set; }

    [JsonProperty("monsterJson")]
    public string MonsterJson { get; set; } = "";
}

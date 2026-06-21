using Newtonsoft.Json;

namespace NahidaImpact.Data.Common;

public class ItemParamData
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("count")]
    public int Count { get; set; }

    public ItemParamData() { }

    public ItemParamData(int id, int count)
    {
        Id = id;
        Count = count;
    }
}
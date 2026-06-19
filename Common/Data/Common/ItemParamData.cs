using System.Text.Json.Serialization;

namespace NahidaImpact.Data.Common;

public class ItemParamData
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("count")]
    public int Count { get; set; }

    public ItemParamData() { }

    public ItemParamData(int id, int count)
    {
        Id = id;
        Count = count;
    }
}
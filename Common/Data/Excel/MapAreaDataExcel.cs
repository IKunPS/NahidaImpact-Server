using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NahidaImpact.Data.Excel;

[ResourceEntity("MapAreaConfigData.json")]
public class MapAreaDataExcel : ExcelResource
{
    [JsonPropertyName("id")]
    public uint Id { get; set; }

    [JsonPropertyName("mapAreaState")]
    public string MapAreaState { get; set; } = string.Empty;

    [JsonPropertyName("areaID1")]
    public List<int> AreaID1 { get; set; } = [];

    [JsonPropertyName("sceneID")]
    public int SceneID { get; set; }

    [JsonPropertyName("scenePointID")]
    public int ScenePointID { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    public override uint GetId() => Id;

    public override void Loaded()
    {
        GameData.MapAreaData[(int)Id] = this;
    }
}

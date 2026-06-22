using Newtonsoft.Json;
using System.Collections.Generic;

namespace NahidaImpact.Data.Excel;

[ResourceEntity("MapAreaConfigData.json")]
public class MapAreaDataExcel : ExcelResource
{
    [JsonProperty("id")]
    public uint Id { get; set; }

    [JsonProperty("mapAreaState")]
    public string MapAreaState { get; set; } = string.Empty;

    [JsonProperty("areaID1")]
    public List<int> AreaID1 { get; set; } = [];

    [JsonProperty("sceneID")]
    public int SceneID { get; set; }

    [JsonProperty("scenePointID")]
    public int ScenePointID { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    public override uint GetId() => Id;

    public override void Loaded()
    {
        GameData.MapAreaData[(int)Id] = this;
    }
}

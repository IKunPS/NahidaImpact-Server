using Newtonsoft.Json;
using System.Collections.Generic;

namespace NahidaImpact.Data.Excel;

[ResourceEntity("MapLayerGroupExcelConfigData.json")]
public class MapLayerGroupDataExcel : ExcelResource
{
    [JsonProperty("id")]
    public uint Id { get; set; }

    [JsonProperty("areaIds")]
    public List<int> AreaIds { get; set; } = [];

    [JsonProperty("mapFloorId")]
    public float MapFloorId { get; set; }

    public override uint GetId() => Id;

    public override void Loaded()
    {
        GameData.MapLayerGroupData[(int)Id] = this;
    }
}

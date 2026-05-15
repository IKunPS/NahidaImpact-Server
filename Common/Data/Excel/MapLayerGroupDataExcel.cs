using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NahidaImpact.Data.Excel;

[ResourceEntity("MapLayerGroupExcelConfigData.json")]
public class MapLayerGroupDataExcel : ExcelResource
{
    [JsonPropertyName("id")]
    public uint Id { get; set; }

    [JsonPropertyName("areaIds")]
    public List<int> AreaIds { get; set; } = [];

    [JsonPropertyName("mapFloorId")]
    public float MapFloorId { get; set; }

    public override uint GetId() => Id;

    public override void Loaded()
    {
        GameData.MapLayerGroupData[(int)Id] = this;
    }
}

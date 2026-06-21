using Newtonsoft.Json;

namespace NahidaImpact.Data.Excel;

[ResourceEntity("MapLayerExcelConfigData.json")]
public class MapLayerDataExcel : ExcelResource
{
    [JsonProperty("id")]
    public uint Id { get; set; }

    [JsonProperty("level")]
    public float Level { get; set; }

    public override uint GetId() => Id;

    public override void Loaded()
    {
        GameData.MapLayerData[(int)Id] = this;
    }
}

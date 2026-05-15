using System.Text.Json.Serialization;

namespace NahidaImpact.Data.Excel;

[ResourceEntity("MapLayerExcelConfigData.json")]
public class MapLayerDataExcel : ExcelResource
{
    [JsonPropertyName("id")]
    public uint Id { get; set; }

    [JsonPropertyName("level")]
    public float Level { get; set; }

    public override uint GetId() => Id;

    public override void Loaded()
    {
        GameData.MapLayerData[(int)Id] = this;
    }
}

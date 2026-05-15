using System.Text.Json.Serialization;

namespace NahidaImpact.Data.Excel;

[ResourceEntity("MapLayerFloorExcelConfigData.json")]
public class MapLayerFloorDataExcel : ExcelResource
{
    [JsonPropertyName("id")]
    public uint Id { get; set; }

    [JsonPropertyName("floorNameTextMapHash")]
    public long FloorNameTextMapHash { get; set; }

    public override uint GetId() => Id;

    public override void Loaded()
    {
        GameData.MapLayerFloorData[(int)Id] = this;
    }
}

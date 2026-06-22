using Newtonsoft.Json;

namespace NahidaImpact.Data.Excel;

[ResourceEntity("MapLayerFloorExcelConfigData.json")]
public class MapLayerFloorDataExcel : ExcelResource
{
    [JsonProperty("id")]
    public uint Id { get; set; }

    [JsonProperty("floorNameTextMapHash")]
    public long FloorNameTextMapHash { get; set; }

    public override uint GetId() => Id;

    public override void Loaded()
    {
        GameData.MapLayerFloorData[(int)Id] = this;
    }
}

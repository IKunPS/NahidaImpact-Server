using Newtonsoft.Json;

namespace NahidaImpact.Data.Excel;

[ResourceEntity("MonsterExcelConfigData.json")]
public class MonsterDataExcel: ExcelResource
{
    [JsonProperty("id")]
    public uint Id { get; set; }

    [JsonProperty("affix")] 
    public List<int> Affix { get; set; } = [];
    
    public override uint GetId() => Id;

    public override void Loaded()
    {
        GameData.MonsterData[(int)Id] = this;
    }
}
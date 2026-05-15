using System.Text.Json.Serialization;

namespace NahidaImpact.Data.Excel;

[ResourceEntity("MonsterExcelConfigData.json")]
public class MonsterDataExcel: ExcelResource
{
    [JsonPropertyName("id")]
    public uint Id { get; set; }

    [JsonPropertyName("affix")] 
    public List<int> Affix { get; set; } = [];
    
    public override uint GetId() => Id;

    public override void Loaded()
    {
        GameData.MonsterData[(int)Id] = this;
    }
}
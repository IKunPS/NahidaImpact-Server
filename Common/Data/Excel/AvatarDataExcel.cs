using System.Text.Json.Serialization;

namespace NahidaImpact.Data.Excel;

[ResourceEntity("AvatarExcelConfigData.json")]
public class AvatarDataExcel : ExcelResource
{
    [JsonPropertyName("id")] public uint Id { get; set; }
    [JsonPropertyName("skillDepotId")] public uint SkillDepotId { get; set; }
    [JsonPropertyName("nameTextMapHash")] public long NameTextMapHash { get; set; } = new();
    [JsonPropertyName("hpBase")] public double HpBase { get; set; }
    [JsonPropertyName("attackBase")] public double AttackBase { get; set; }
    [JsonPropertyName("defenseBase")] public double DefenseBase { get; set; }
    [JsonPropertyName("chargeEfficiency")] public double ChargeEfficiency { get; set; }
    
    [JsonPropertyName("critical")] public double Critical { get; set; }
    [JsonPropertyName("criticalHurt")] public double CriticalHurt { get; set; }
    [JsonPropertyName("initialWeapon")] public uint InitialWeapon { get; set; }

    public override uint GetId()
    {
        return Id;
    }

    public override void Loaded()
    {
        GameData.AvatarData.Add((int)Id, this);   
    }
}
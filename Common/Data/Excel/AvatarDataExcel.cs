using System.Text.Json.Serialization;
using NahidaImpact.Util;

namespace NahidaImpact.Data.Excel;

[ResourceEntity("AvatarExcelConfigData.json")]
public class AvatarDataExcel : ExcelResource
{
    [JsonPropertyName("id")] public uint Id { get; set; }
    [JsonPropertyName("skillDepotId")] public uint SkillDepotId { get; set; }
    [JsonPropertyName("nameTextMapHash")] public long NameTextMapHash { get; set; } = new();
    [JsonPropertyName("iconName")] public string IconName { get; set; } = "";
    [JsonPropertyName("hpBase")] public double HpBase { get; set; }
    [JsonPropertyName("attackBase")] public double AttackBase { get; set; }
    [JsonPropertyName("defenseBase")] public double DefenseBase { get; set; }
    [JsonPropertyName("chargeEfficiency")] public double ChargeEfficiency { get; set; }
    
    [JsonPropertyName("critical")] public double Critical { get; set; }
    [JsonPropertyName("criticalHurt")] public double CriticalHurt { get; set; }
    [JsonPropertyName("initialWeapon")] public uint InitialWeapon { get; set; }

    public List<uint> Abilities { get; set; } = [];
    public List<string> AbilityNames { get; set; } = [];

    // Element type: 1=Fire, 2=Water, 3=Wind, 4=Electric, 5=Grass, 6=Ice, 7=Rock, 0=Default
    public int ElementType { get; set; }

    /// <summary>Avatar short name extracted from IconName (e.g. "Qin" from "UI_AvatarIcon_Qin").</summary>
    public string Name { get; set; } = "";

    public override uint GetId()
    {
        return Id;
    }

    public override void Loaded()
    {
        GameData.AvatarData.Add((int)Id, this);
    }

    public override void AfterAllDone()
    {
        BuildEmbryo();
    }

    /// <summary>
    /// Build ability embryo list from BinOutput/Avatar/*.json.
    /// Matches Java AvatarData.buildEmbryo().
    /// </summary>
    public void BuildEmbryo()
    {
        var split = IconName.Split('_');
        if (split.Length == 0) return;

        Name = split[^1]; // last segment

        if (!GameData.AvatarConfigData.TryGetValue(Name, out var avatarConfig))
        {
            ResourceManager.Logger.Warn($"Avatar config not found for '{Name}' (icon: {IconName})");
            return;
        }
        if (avatarConfig.Abilities == null || avatarConfig.Abilities.Count == 0) return;

        foreach (var ab in avatarConfig.Abilities)
        {
            if (!string.IsNullOrEmpty(ab.AbilityName))
            {
                Abilities.Add(Utils.AbilityHash(ab.AbilityName));
                AbilityNames.Add(ab.AbilityName);
            }
        }
    }
}
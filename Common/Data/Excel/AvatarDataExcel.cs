using System.Text.Json.Serialization;
using NahidaImpact.Data.Common;
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
    [JsonPropertyName("avatarPromoteId")] public int AvatarPromoteId { get; set; }

    [JsonPropertyName("propGrowCurves")] public List<PropGrowCurveData> PropGrowCurves { get; set; } = [];

    [JsonIgnore] public float[] HpGrowthCurve { get; private set; } = [];
    [JsonIgnore] public float[] AtkGrowthCurve { get; private set; } = [];
    [JsonIgnore] public float[] DefGrowthCurve { get; private set; } = [];

    public List<uint> Abilities { get; set; } = [];
    public List<string> AbilityNames { get; set; } = [];
    public string Name { get; set; } = "";

    public int ElementType { get; set; }

    public override uint GetId() => Id;

    public override void Loaded()
    {
        GameData.AvatarData.Add((int)Id, this);
    }

    public override void AfterAllDone()
    {
        BuildEmbryo();
        BuildGrowthCurves();
    }

    /// <summary>Build per-level stat growth multiplier arrays from AvatarCurveData.</summary>
    private void BuildGrowthCurves()
    {
        if (PropGrowCurves.Count == 0) return;

        int size = GameData.AvatarCurveData.Count;
        if (size == 0) return;

        HpGrowthCurve = new float[size];
        AtkGrowthCurve = new float[size];
        DefGrowthCurve = new float[size];

        foreach (var (_, curveData) in GameData.AvatarCurveData)
        {
            int idx = curveData.Level - 1;
            if (idx < 0 || idx >= size) continue;

            foreach (var growCurve in PropGrowCurves)
            {
                foreach (var info in curveData.CurveInfos)
                {
                    if (info.Type != growCurve.GrowCurve) continue;

                    if (growCurve.Type == "FIGHT_PROP_BASE_HP")
                        HpGrowthCurve[idx] = info.Value;
                    else if (growCurve.Type == "FIGHT_PROP_BASE_ATTACK")
                        AtkGrowthCurve[idx] = info.Value;
                    else if (growCurve.Type == "FIGHT_PROP_BASE_DEFENSE")
                        DefGrowthCurve[idx] = info.Value;
                }
            }
        }
    }

    public float GetBaseHp(int level)
    {
        int idx = level - 1;
        float curve = idx >= 0 && idx < HpGrowthCurve.Length ? HpGrowthCurve[idx] : 1f;
        return (float)HpBase * curve;
    }

    public float GetBaseAttack(int level)
    {
        int idx = level - 1;
        float curve = idx >= 0 && idx < AtkGrowthCurve.Length ? AtkGrowthCurve[idx] : 1f;
        return (float)AttackBase * curve;
    }

    public float GetBaseDefense(int level)
    {
        int idx = level - 1;
        float curve = idx >= 0 && idx < DefGrowthCurve.Length ? DefGrowthCurve[idx] : 1f;
        return (float)DefenseBase * curve;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NahidaImpact.Data.Excel;

[ResourceEntity("MonsterExcelConfigData.json")]
public class MonsterDataExcel : ExcelResource
{
    [JsonProperty("id")] public uint Id { get; set; }
    [JsonProperty("monsterName")] public string MonsterName { get; set; } = "";
    [JsonProperty("type")] public string MonsterType { get; set; } = "NORMAL";
    [JsonProperty("securityLevel")] public string SecurityLevel { get; set; } = "";
    [JsonProperty("ai")] public string Ai { get; set; } = "";

    // Base stats
    [JsonProperty("hpBase")] public float HpBase { get; set; } = 1000f;
    [JsonProperty("attackBase")] public float AttackBase { get; set; } = 100f;
    [JsonProperty("defenseBase")] public float DefenseBase { get; set; } = 500f;
    [JsonProperty("critical")] public float Critical { get; set; } = 0.05f;
    [JsonProperty("criticalHurt")] public float CriticalHurt { get; set; } = 0.5f;
    [JsonProperty("elementMastery")] public float ElementMastery { get; set; }
    [JsonProperty("hpPercent")] public float HpPercent { get; set; } = 100f;
    // hpDrops can be either an array of floats or a dict of string→float
    [JsonProperty("hpDrops")] public JToken HpDropsRaw { get; set; } = JValue.CreateNull();

    [JsonIgnore]
    public List<float> HpDrops => HpDropsRaw switch
    {
        JArray arr => arr.Select(v => (float)v).ToList(),
        JObject obj => obj.Properties().Select(p => (float)(p.Value ?? 0f)).ToList(),
        _ => []
    };

    // Elemental resistances
    [JsonProperty("fireSubHurt")] public float FireSubHurt { get; set; }
    [JsonProperty("waterSubHurt")] public float WaterSubHurt { get; set; }
    [JsonProperty("grassSubHurt")] public float GrassSubHurt { get; set; }
    [JsonProperty("elecSubHurt")] public float ElecSubHurt { get; set; }
    [JsonProperty("iceSubHurt")] public float IceSubHurt { get; set; }
    [JsonProperty("windSubHurt")] public float WindSubHurt { get; set; }
    [JsonProperty("rockSubHurt")] public float RockSubHurt { get; set; }
    [JsonProperty("physicalSubHurt")] public float PhysicalSubHurt { get; set; }

    // Scaling
    [JsonProperty("growCurve")] public string GrowCurve { get; set; } = "";
    [JsonProperty("propGrowCurves")] public List<MonsterPropGrowCurve> PropGrowCurves { get; set; } = [];

    // Other
    [JsonProperty("affix")] public List<int> Affix { get; set; } = [];
    [JsonProperty("campID")] public int CampId { get; set; }
    [JsonProperty("dropId")] public int DropId { get; set; }
    [JsonProperty("killDropId")] public int KillDropId { get; set; }
    [JsonProperty("mpPropID")] public int MpPropId { get; set; }
    [JsonProperty("playType")] public string PlayType { get; set; } = "";
    [JsonProperty("skin")] public string Skin { get; set; } = "";
    [JsonProperty("subType")] public string SubType { get; set; } = "";
    [JsonProperty("describeId")] public int DescribeId { get; set; }
    [JsonProperty("combatBGMLevel")] public int CombatBgmLevel { get; set; } = 1;
    [JsonProperty("entityBudgetLevel")] public int EntityBudgetLevel { get; set; } = 1;
    [JsonProperty("featureTagGroupID")] public int FeatureTagGroupId { get; set; }
    [JsonProperty("radarHintID")] public int RadarHintId { get; set; }

    public bool IsElite => MonsterType is "ELITE" or "BOSS";
    public bool IsBoss => MonsterType == "BOSS";

    public override uint GetId() => Id;

    public override void Loaded()
    {
        GameData.MonsterData[(int)Id] = this;
    }

    // Get level-scaled property using propGrowCurves
    public float GetScaledProp(int level, MonsterGrowCurveType curveType)
    {
        var curve = PropGrowCurves.Find(c => c.CurveType == curveType);
        if (curve == null) return curveType switch
        {
            MonsterGrowCurveType.Hp => HpBase * level,
            MonsterGrowCurveType.Attack => AttackBase * level,
            MonsterGrowCurveType.Defense => DefenseBase * level,
            _ => 0f
        };
        return curve.GrowCurve;
    }
}

public class MonsterPropGrowCurve
{
    // JSON uses FIGHT_PROP strings, not the MonsterGrowCurveType enum names
    [JsonProperty("type")] public string TypeName { get; set; } = "";

    [JsonIgnore]
    public MonsterGrowCurveType CurveType => TypeName switch
    {
        "FIGHT_PROP_BASE_HP" => MonsterGrowCurveType.Hp,
        "FIGHT_PROP_HP_PERCENT" => MonsterGrowCurveType.HpRatio,
        "FIGHT_PROP_BASE_ATTACK" => MonsterGrowCurveType.Attack,
        "FIGHT_PROP_ATTACK_PERCENT" => MonsterGrowCurveType.AttackRatio,
        "FIGHT_PROP_BASE_DEFENSE" => MonsterGrowCurveType.Defense,
        "FIGHT_PROP_DEFENSE_PERCENT" => MonsterGrowCurveType.DefenseRatio,
        "FIGHT_PROP_BASE_SPEED" or "FIGHT_PROP_SPEED_PERCENT" => MonsterGrowCurveType.Speed,
        "FIGHT_PROP_ELEMENT_MASTERY" => MonsterGrowCurveType.ElementMastery,
        "FIGHT_PROP_CRITICAL" => MonsterGrowCurveType.Critical,
        "FIGHT_PROP_CRITICAL_HURT" => MonsterGrowCurveType.CriticalHurt,
        "FIGHT_PROP_FIRE_SUB_HURT" => MonsterGrowCurveType.FireSubHurt,
        "FIGHT_PROP_WATER_SUB_HURT" => MonsterGrowCurveType.WaterSubHurt,
        "FIGHT_PROP_GRASS_SUB_HURT" => MonsterGrowCurveType.GrassSubHurt,
        "FIGHT_PROP_ELEC_SUB_HURT" => MonsterGrowCurveType.ElecSubHurt,
        "FIGHT_PROP_ICE_SUB_HURT" => MonsterGrowCurveType.IceSubHurt,
        "FIGHT_PROP_WIND_SUB_HURT" => MonsterGrowCurveType.WindSubHurt,
        "FIGHT_PROP_ROCK_SUB_HURT" => MonsterGrowCurveType.RockSubHurt,
        "FIGHT_PROP_PHYSICAL_SUB_HURT" => MonsterGrowCurveType.PhysicalSubHurt,
        "FIGHT_PROP_FIRE_ADD_HURT" => MonsterGrowCurveType.FireAddHurt,
        "FIGHT_PROP_WATER_ADD_HURT" => MonsterGrowCurveType.WaterAddHurt,
        "FIGHT_PROP_GRASS_ADD_HURT" => MonsterGrowCurveType.GrassAddHurt,
        "FIGHT_PROP_ELEC_ADD_HURT" => MonsterGrowCurveType.ElecAddHurt,
        "FIGHT_PROP_ICE_ADD_HURT" => MonsterGrowCurveType.IceAddHurt,
        "FIGHT_PROP_WIND_ADD_HURT" => MonsterGrowCurveType.WindAddHurt,
        "FIGHT_PROP_ROCK_ADD_HURT" => MonsterGrowCurveType.RockAddHurt,
        "FIGHT_PROP_PHYSICAL_ADD_HURT" => MonsterGrowCurveType.PhysicalAddHurt,
        _ => MonsterGrowCurveType.Hp
    };

    // growCurve is either a float (direct scale) or a string (curve name reference)
    [JsonProperty("growCurve")] public JToken GrowCurveRaw { get; set; } = JValue.CreateNull();

    [JsonIgnore]
    public float GrowCurve => GrowCurveRaw switch
    {
        JValue { Type: JTokenType.Float or JTokenType.Integer } v => (float)v,
        JValue { Type: JTokenType.String } => 1f,
        _ => 1f
    };
}

public enum MonsterGrowCurveType
{
    Hp,
    Attack,
    Defense,
    HpRatio,
    AttackRatio,
    DefenseRatio,
    Speed,
    ElementMastery,
    Critical,
    CriticalHurt,
    FireSubHurt,
    WaterSubHurt,
    GrassSubHurt,
    ElecSubHurt,
    IceSubHurt,
    WindSubHurt,
    RockSubHurt,
    PhysicalSubHurt,
    FireAddHurt,
    WaterAddHurt,
    GrassAddHurt,
    ElecAddHurt,
    IceAddHurt,
    WindAddHurt,
    RockAddHurt,
    PhysicalAddHurt,
}

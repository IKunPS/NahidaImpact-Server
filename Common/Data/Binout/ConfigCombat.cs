namespace NahidaImpact.Data.Binout;

/// <summary>Combat config for entities. Mirrors Java ConfigCombat.</summary>
public class ConfigCombat
{
    public ConfigCombatProperty? Property { get; set; }
    public ConfigCombatSummon? Summon { get; set; }
}

/// <summary>Combat property overrides. Mirrors Java ConfigCombatProperty.</summary>
public class ConfigCombatProperty
{
    public float Hp { get; set; }
    public float Attack { get; set; }
    public float Defense { get; set; }
    public float HpPercent { get; set; }
    public float AttackPercent { get; set; }
    public float DefensePercent { get; set; }
}

/// <summary>Combat summon config. Mirrors Java ConfigCombatSummon.</summary>
public class ConfigCombatSummon { }

namespace NahidaImpact.Data.Binout;

public class ConfigCombat
{
    public ConfigCombatProperty? Property { get; set; }
    public ConfigCombatSummon? Summon { get; set; }
}

public class ConfigCombatProperty
{
    public float Hp { get; set; }
    public float Attack { get; set; }
    public float Defense { get; set; }
    public float HpPercent { get; set; }
    public float AttackPercent { get; set; }
    public float DefensePercent { get; set; }
}

public class ConfigCombatSummon { }

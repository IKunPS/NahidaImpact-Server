namespace NahidaImpact.Data.Binout;

public class ConfigEntityBase
{
    public ConfigCommon? ConfigCommon { get; set; }
    public ConfigCombat? Combat { get; set; }
    public List<ConfigAbilityData>? Abilities { get; set; }
    public ConfigGlobalValue? GlobalValue { get; set; }
}

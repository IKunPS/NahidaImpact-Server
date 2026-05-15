namespace NahidaImpact.Data.Binout;

/// <summary>
/// Base class for entity configs loaded from BinOutput.
/// Mirrors Java ConfigEntityBase.
/// </summary>
public class ConfigEntityBase
{
    public ConfigCommon? ConfigCommon { get; set; }
    public ConfigCombat? Combat { get; set; }
    public List<ConfigAbilityData>? Abilities { get; set; }
    public ConfigGlobalValue? GlobalValue { get; set; }
}

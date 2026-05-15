namespace NahidaImpact.Data.Binout;

/// <summary>
/// References an ability by name or ID. Mirrors Java ConfigAbilityData.
/// </summary>
public class ConfigAbilityData
{
    public string AbilityId { get; set; } = "";
    public string AbilityName { get; set; } = "";
    public string AbilityOverride { get; set; } = "";
}

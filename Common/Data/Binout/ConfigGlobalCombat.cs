namespace NahidaImpact.Data.Binout;

/// <summary>
/// Global combat configuration loaded from BinOutput/Common/ConfigGlobalCombat.json.
/// Mirrors Java ConfigGlobalCombat.
/// </summary>
public class ConfigGlobalCombat
{
    public DefaultAbilities? DefaultAbilities { get; set; }
}

public class DefaultAbilities
{
    /// <summary>Monster elite ability name (Java field name preserved)</summary>
    public string MonterEliteAbilityName { get; set; } = "";
    public List<string>? NonHumanoidMoveAbilities { get; set; }
    public List<string>? LevelDefaultAbilities { get; set; }
    public List<string>? LevelElementAbilities { get; set; }
    public List<string>? LevelItemAbilities { get; set; }
    public List<string>? LevelSBuffAbilities { get; set; }
    public List<string>? DefaultMpLevelAbilities { get; set; }
    public List<string>? DefaultAvatarAbilities { get; set; }
    public List<string>? DefaultTeamAbilities { get; set; }
}

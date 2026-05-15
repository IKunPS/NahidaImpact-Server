namespace NahidaImpact.Data.Binout;

/// <summary>
/// Level entity configuration loaded from BinOutput/LevelEntity/*.json.
/// Mirrors Java ConfigLevelEntity.
/// </summary>
public class ConfigLevelEntity
{
    public List<ConfigAbilityData>? Abilities { get; set; }
    public List<ConfigAbilityData>? MonsterAbilities { get; set; }
    public List<ConfigAbilityData>? AvatarAbilities { get; set; }
    public List<ConfigAbilityData>? TeamAbilities { get; set; }
    public List<int>? PreloadMonsterEntityIds { get; set; }
    public string DropElemControlType { get; set; } = "";
}

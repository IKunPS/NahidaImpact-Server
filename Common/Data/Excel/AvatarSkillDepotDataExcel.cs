using System.Text.Json.Serialization;
using NahidaImpact.Util;

namespace NahidaImpact.Data.Excel;

/// <summary>
/// Avatar skill depot config. Mirrors Java AvatarSkillDepotData.
/// Loaded from AvatarSkillDepotExcelConfigData.json.
/// </summary>
[ResourceEntity("AvatarSkillDepotExcelConfigData.json")]
public class AvatarSkillDepotDataExcel : ExcelResource
{
    [JsonPropertyName("id")]
    public uint Id { get; set; }

    [JsonPropertyName("energySkill")]
    public uint EnergySkill { get; set; }

    [JsonPropertyName("attackModeSkill")]
    public uint AttackModeSkill { get; set; }

    [JsonPropertyName("skills")]
    public List<uint> Skills { get; set; } = [];

    [JsonPropertyName("subSkills")]
    public List<uint> SubSkills { get; set; } = [];

    [JsonPropertyName("extraAbilities")]
    public List<string> ExtraAbilities { get; set; } = [];

    /// <summary>Ability hash list computed from skill depot ability group. Matches Java getAbilities().</summary>
    public List<uint> AbilityHashes { get; set; } = [];

    [JsonPropertyName("talents")]
    public List<uint> Talents { get; set; } = [];

    [JsonPropertyName("talentStarName")]
    public string TalentStarName { get; set; } = "";

    [JsonPropertyName("skillDepotAbilityGroup")]
    public string SkillDepotAbilityGroup { get; set; } = "";

    /// <summary>Element type resolved from energy skill data. 0=None, 1=Fire, 2=Water, etc.</summary>
    public int ElementType { get; set; }

    public override uint GetId() => Id;

    public override void Loaded()
    {
        GameData.AvatarSkillDepotData[(int)Id] = this;
    }

    public override void AfterAllDone()
    {
        // Resolve element type from energy skill's cost element (runs after all Excel data is loaded)
        if (EnergySkill > 0 && GameData.AvatarSkillData.TryGetValue((int)EnergySkill, out var skillData))
        {
            ElementType = skillData.ElementTypeValue;
        }

        // Look up skill depot ability group in PlayerAbilities (loaded from BinOutput/AbilityGroup/*.json)
        // Mirrors Java AvatarSkillDepotData.onLoad() — GameDepot.getPlayerAbilities().get()
        if (!string.IsNullOrEmpty(SkillDepotAbilityGroup)
            && GameData.PlayerAbilities.TryGetValue(SkillDepotAbilityGroup, out var abilityGroup))
        {
            if (abilityGroup.Abilities != null)
            {
                foreach (var ab in abilityGroup.Abilities)
                {
                    if (!string.IsNullOrEmpty(ab.AbilityName))
                        AbilityHashes.Add(Utils.AbilityHash(ab.AbilityName));
                }
            }
        }

        // Also hash extra abilities from Excel config
        foreach (var ab in ExtraAbilities)
        {
            if (!string.IsNullOrEmpty(ab))
                AbilityHashes.Add(Utils.AbilityHash(ab));
        }
    }
}

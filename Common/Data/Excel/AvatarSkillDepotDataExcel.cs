using Newtonsoft.Json;
using NahidaImpact.Util;

namespace NahidaImpact.Data.Excel;

/// <summary>
/// Avatar skill depot config. Mirrors Java AvatarSkillDepotData.
/// Loaded from AvatarSkillDepotExcelConfigData.json.
/// </summary>
[ResourceEntity("AvatarSkillDepotExcelConfigData.json")]
public class AvatarSkillDepotDataExcel : ExcelResource
{
    [JsonProperty("id")]
    public uint Id { get; set; }

    [JsonProperty("energySkill")]
    public uint EnergySkill { get; set; }

    [JsonProperty("attackModeSkill")]
    public uint AttackModeSkill { get; set; }

    [JsonProperty("skills")]
    public List<uint> Skills { get; set; } = [];

    [JsonProperty("subSkills")]
    public List<uint> SubSkills { get; set; } = [];

    [JsonProperty("extraAbilities")]
    public List<string> ExtraAbilities { get; set; } = [];

    /// <summary>Ability hash list computed from skill depot ability group. Matches Java getAbilities().</summary>
    public List<uint> AbilityHashes { get; set; } = [];

    [JsonProperty("talents")]
    public List<uint> Talents { get; set; } = [];

    [JsonProperty("talentStarName")]
    public string TalentStarName { get; set; } = "";

    [JsonProperty("skillDepotAbilityGroup")]
    public string SkillDepotAbilityGroup { get; set; } = "";

    [JsonProperty("inherentProudSkillOpens")]
    public List<InherentProudSkillOpen> InherentProudSkillOpens { get; set; } = [];

    [JsonProperty("specialProudSkillOpens")]
    public List<SpecialProudSkillOpen> SpecialProudSkillOpens { get; set; } = [];

    /// <summary>Element type resolved from energy skill data. 0=None, 1=Fire, 2=Water, etc.</summary>
    public int ElementType { get; set; }

    /// <summary>命座消耗道具ID，从第一个talent解析</summary>
    public int TalentCostItemId { get; set; }

    public override uint GetId() => Id;

    public override void Loaded()
    {
        GameData.AvatarSkillDepotData[(int)Id] = this;
    }

    public override void AfterAllDone()
    {
        if (EnergySkill > 0 && GameData.AvatarSkillData.TryGetValue((int)EnergySkill, out var skillData))
        {
            ElementType = skillData.ElementTypeValue;
        }

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

        foreach (var ab in ExtraAbilities)
        {
            if (!string.IsNullOrEmpty(ab))
                AbilityHashes.Add(Utils.AbilityHash(ab));
        }

        // 从第一个talent解析命座消耗道具ID
        if (Talents.Count > 0 && GameData.AvatarTalentData.TryGetValue((int)Talents[0], out var talentData))
        {
            TalentCostItemId = talentData.MainCostItemId;
        }
    }

    /// <summary>获取所有技能+能量技能的ID列表</summary>
    public IEnumerable<uint> GetSkillsAndEnergySkill()
    {
        foreach (var skillId in Skills)
            yield return skillId;
        if (EnergySkill > 0)
            yield return EnergySkill;
    }

    public class InherentProudSkillOpen
    {
        [JsonProperty("proudSkillGroupId")] public int ProudSkillGroupId { get; set; }
        [JsonProperty("needAvatarPromoteLevel")] public int NeedAvatarPromoteLevel { get; set; }
    }

    public class SpecialProudSkillOpen
    {
        [JsonProperty("proudSkillGroupId")] public int ProudSkillGroupId { get; set; }
    }
}
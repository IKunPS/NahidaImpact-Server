using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace NahidaImpact.Data.Ability;

public class AbilityMixinData
{
    public enum MixinType
    {
        AttachToGadgetStateMixin,
        AttachToStateIDMixin,
        ShieldBarMixin,
        SwitchHealToHPDebtsMixin,
        AttachModifierToGlobalValueMixin,
        DoActionOnGlobalValueChangeMixin,
        CurLocalAvatarMixin,
        NyxCostMixin,
        ModifyDamageMixin,
        AvatarChangeSkillMixin,
        PhlogistonCostMixin,
        AttachModifierToSelfGlobalValueMixin,
        AttachActionToModifierMixin,
        AttachModifierToSelfGlobalValueNoInitMixin,
        HPDebtsMixin,
        LimitHpDebtsByTagMixin,
        TileAttackManagerMixin,
        CostStaminaMixin,
        DoActionByEnergyChangeMixin,
        RejectAttackMixin,
        DoActionByTargetsCountMixin,
        AttachToAbilityStateMixin,
        ModifyBeHitDamageMixin,
        MuteHitEffectMixin,
        EntityInVisibleMixin,
        TriggerPostProcessEffectMixin,
        DoActionByKillingMixin,
        AvatarSteerByCameraMixin,
        ModifyDamageCountMixin,
        OnAvatarUseSkillMixin,
        ReviveElemEnergyMixin,
    }

    [JsonProperty("$type")]
    public MixinType Type { get; set; }

    /// <summary>Can be a string or array of strings. Handled by ModifierNames.</summary>
    [JsonProperty("modifierName")]
    public JToken? ModifierName { get; set; }

    [JsonProperty("stateIDs")]
    public List<string> StateIDs { get; set; } = [];

    [JsonProperty("globalValueKey")]
    public string GlobalValueKey { get; set; } = string.Empty;

    public float Speed { get; set; }

    public float CostStaminaDelta { get; set; }

    public float Ratio { get; set; } = 1.0f;

    public float DefaultGlobalValueOnCreate { get; set; }

    public List<float> RatioSteps { get; set; } = [];

    public List<string> ModifierNameSteps { get; set; } = [];

    public List<string> ModifierNames
    {
        get
        {
            if (ModifierName == null) return new List<string>();
            if (ModifierName.Type == JTokenType.Array)
                return ModifierName.ToObject<List<string>>() ?? new List<string>();
            return new List<string> { ModifierName.ToString() };
        }
    }
}
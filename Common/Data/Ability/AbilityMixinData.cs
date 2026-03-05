using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Text.Json;

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

    [JsonPropertyName("$type")]
    public MixinType Type { get; set; }

    [JsonPropertyName("modifierName")]
    public JsonElement ModifierName { get; set; }

    [JsonPropertyName("stateIDs")]
    public List<string> StateIDs { get; set; } = [];

    [JsonPropertyName("globalValueKey")]
    public string GlobalValueKey { get; set; } = string.Empty;

    [JsonPropertyName("speed")]
    public float Speed { get; set; }

    [JsonPropertyName("costStaminaDelta")]
    public float CostStaminaDelta { get; set; }

    [JsonPropertyName("ratio")]
    public float Ratio { get; set; } = 1.0f;

    [JsonPropertyName("defaultGlobalValueOnCreate")]
    public float DefaultGlobalValueOnCreate { get; set; }

    [JsonPropertyName("ratioSteps")]
    public List<float> RatioSteps { get; set; } = [];

    [JsonPropertyName("modifierNameSteps")]
    public List<string> ModifierNameSteps { get; set; } = [];

    public List<string> GetModifierNames()
    {
        if (ModifierName.ValueKind == JsonValueKind.Array)
        {
            return JsonSerializer.Deserialize<List<string>>(ModifierName.GetRawText());
        }
        else
        {
            return new List<string> { ModifierName.GetString() };
        }
    }
}
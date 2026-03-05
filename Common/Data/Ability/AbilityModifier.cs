using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace NahidaImpact.Data.Ability;

public class AbilityModifier
{
    [JsonPropertyName("state")]
    public State State { get; set; }

    [JsonPropertyName("onAdded")]
    public AbilityModifierAction[] OnAdded { get; set; } = [];

    [JsonPropertyName("onThinkInterval")]
    public AbilityModifierAction[] OnThinkInterval { get; set; } = [];

    [JsonPropertyName("onRemoved")]
    public AbilityModifierAction[] OnRemoved { get; set; } = [];

    [JsonPropertyName("onBeingHit")]
    public AbilityModifierAction[] OnBeingHit { get; set; } = [];

    [JsonPropertyName("onAttackLanded")]
    public AbilityModifierAction[] OnAttackLanded { get; set; } = [];

    [JsonPropertyName("onHittingOther")]
    public AbilityModifierAction[] OnHittingOther { get; set; } = [];

    [JsonPropertyName("onKill")]
    public AbilityModifierAction[] OnKill { get; set; } = [];

    [JsonPropertyName("onCrash")]
    public AbilityModifierAction[] OnCrash { get; set; } = [];

    [JsonPropertyName("onAvatarIn")]
    public AbilityModifierAction[] OnAvatarIn { get; set; } = [];

    [JsonPropertyName("onAvatarOut")]
    public AbilityModifierAction[] OnAvatarOut { get; set; } = [];

    [JsonPropertyName("onReconnect")]
    public AbilityModifierAction[] OnReconnect { get; set; } = [];

    [JsonPropertyName("onChangeAuthority")]
    public AbilityModifierAction[] OnChangeAuthority { get; set; } = [];

    [JsonPropertyName("onVehicleIn")]
    public AbilityModifierAction[] OnVehicleIn { get; set; } = [];

    [JsonPropertyName("onVehicleOut")]
    public AbilityModifierAction[] OnVehicleOut { get; set; } = [];

    [JsonPropertyName("onZoneEnter")]
    public AbilityModifierAction[] OnZoneEnter { get; set; } = [];

    [JsonPropertyName("onZoneExit")]
    public AbilityModifierAction[] OnZoneExit { get; set; } = [];

    [JsonPropertyName("onHeal")]
    public AbilityModifierAction[] OnHeal { get; set; } = [];

    [JsonPropertyName("onBeingHealed")]
    public AbilityModifierAction[] OnBeingHealed { get; set; } = [];

    [JsonPropertyName("duration")]
    public float Duration { get; set; }

    [JsonPropertyName("thinkInterval")]
    public float ThinkInterval { get; set; }

    [JsonPropertyName("stacking")]
    public string Stacking { get; set; } = string.Empty;

    [JsonPropertyName("modifierMixins")]
    public AbilityMixinData[] ModifierMixins { get; set; } = [];

    [JsonPropertyName("modifierName")]
    public string ModifierName { get; set; } = string.Empty;

    [JsonPropertyName("properties")]
    public AbilityModifierProperty Properties { get; set; } = new();

    [JsonPropertyName("elementType")]
    public ElementType ElementType { get; set; }

    [JsonPropertyName("elementDurability")]
    public float ElementDurability { get; set; }
}

public class AbilityModifierProperty
{
    [JsonPropertyName("Actor_HpThresholdRatio")]
    public float ActorHpThresholdRatio { get; set; }

    [JsonPropertyName("Actor_MaxHPRatio")]
    public float ActorMaxHPRatio { get; set; }

    [JsonPropertyName("Actor_AttackSRatio")]
    public float ActorAttackSRatio { get; set; }

    [JsonPropertyName("Actor_HealedAddDelta")]
    public float ActorHealedAddDelta { get; set; }
}

public enum State
{
    LockHP,
    Invincible,
    ElementFreeze,
    ElementPetrifaction,
    DenyLockOn,
    Limbo,
    NoHeal,
    IgnoreAddEnergy,
    IsGhostToEnemy,
    IsGhostToAllied,
    UnlockFrequencyLimit,
    AttackUp,
    DefenseDown,
    ElementDeadTime,
    SpeedUp,
    DefenseUp,
    Struggle,
    OvergrowVariation,
    ElementElectric,
    ElementFire,
    NyxState,
    ElementBurning,
    ElementShock,
    ElementWet,
    ElementIce,
    ElementFrozen,
    ElementRock,
    ElementWind,
    ElementGrass,
    ElementOverdose,
    SpeedDown,
    MuteTaunt
}

public enum ElementType
{
    None,
    Fire,
    Water,
    Grass,
    Electric,
    Ice,
    Frozen,
    Wind,
    Rock,
    Physical
}
using Newtonsoft.Json;
using System.Collections.Generic;

namespace NahidaImpact.Data.Ability;

public class AbilityModifier
{
    [JsonProperty("state")]
    public State State { get; set; }

    [JsonProperty("onAdded")]
    public AbilityModifierAction[] OnAdded { get; set; } = [];

    [JsonProperty("onThinkInterval")]
    public AbilityModifierAction[] OnThinkInterval { get; set; } = [];

    [JsonProperty("onRemoved")]
    public AbilityModifierAction[] OnRemoved { get; set; } = [];

    [JsonProperty("onBeingHit")]
    public AbilityModifierAction[] OnBeingHit { get; set; } = [];

    [JsonProperty("onAttackLanded")]
    public AbilityModifierAction[] OnAttackLanded { get; set; } = [];

    [JsonProperty("onHittingOther")]
    public AbilityModifierAction[] OnHittingOther { get; set; } = [];

    [JsonProperty("onKill")]
    public AbilityModifierAction[] OnKill { get; set; } = [];

    [JsonProperty("onCrash")]
    public AbilityModifierAction[] OnCrash { get; set; } = [];

    [JsonProperty("onAvatarIn")]
    public AbilityModifierAction[] OnAvatarIn { get; set; } = [];

    [JsonProperty("onAvatarOut")]
    public AbilityModifierAction[] OnAvatarOut { get; set; } = [];

    [JsonProperty("onReconnect")]
    public AbilityModifierAction[] OnReconnect { get; set; } = [];

    [JsonProperty("onChangeAuthority")]
    public AbilityModifierAction[] OnChangeAuthority { get; set; } = [];

    [JsonProperty("onVehicleIn")]
    public AbilityModifierAction[] OnVehicleIn { get; set; } = [];

    [JsonProperty("onVehicleOut")]
    public AbilityModifierAction[] OnVehicleOut { get; set; } = [];

    [JsonProperty("onZoneEnter")]
    public AbilityModifierAction[] OnZoneEnter { get; set; } = [];

    [JsonProperty("onZoneExit")]
    public AbilityModifierAction[] OnZoneExit { get; set; } = [];

    [JsonProperty("onHeal")]
    public AbilityModifierAction[] OnHeal { get; set; } = [];

    [JsonProperty("onBeingHealed")]
    public AbilityModifierAction[] OnBeingHealed { get; set; } = [];

    [JsonProperty("duration")]
    public float Duration { get; set; }

    [JsonProperty("thinkInterval")]
    public float ThinkInterval { get; set; }

    [JsonProperty("stacking")]
    public string Stacking { get; set; } = string.Empty;

    [JsonProperty("modifierMixins")]
    public AbilityMixinData[] ModifierMixins { get; set; } = [];

    [JsonProperty("modifierName")]
    public string ModifierName { get; set; } = string.Empty;

    [JsonProperty("properties")]
    public AbilityModifierProperty Properties { get; set; } = new();

    [JsonProperty("elementType")]
    public ElementType ElementType { get; set; }

    [JsonProperty("elementDurability")]
    public float ElementDurability { get; set; }
}

public class AbilityModifierProperty
{
    [JsonProperty("Actor_HpThresholdRatio")]
    public float ActorHpThresholdRatio { get; set; }

    [JsonProperty("Actor_MaxHPRatio")]
    public float ActorMaxHPRatio { get; set; }

    [JsonProperty("Actor_AttackSRatio")]
    public float ActorAttackSRatio { get; set; }

    [JsonProperty("Actor_HealedAddDelta")]
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
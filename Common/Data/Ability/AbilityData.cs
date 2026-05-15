using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace NahidaImpact.Data.Ability;

public class AbilityData
{
    [JsonPropertyName("abilityName")]
    public string AbilityName { get; set; } = string.Empty;

    [JsonPropertyName("modifiers")]
    public Dictionary<string, AbilityModifier> Modifiers { get; set; } = new();

    [JsonPropertyName("isDynamicAbility")]
    public bool IsDynamicAbility { get; set; }

    [JsonPropertyName("abilitySpecials")]
    public Dictionary<string, float> AbilitySpecials { get; set; } = new();

    [JsonPropertyName("onAdded")]
    public AbilityModifierAction[] OnAdded { get; set; } = [];

    [JsonPropertyName("onRemoved")]
    public AbilityModifierAction[] OnRemoved { get; set; } = [];

    [JsonPropertyName("onAbilityStart")]
    public AbilityModifierAction[] OnAbilityStart { get; set; } = [];

    [JsonPropertyName("onKill")]
    public AbilityModifierAction[] OnKill { get; set; } = [];

    [JsonPropertyName("onFieldEnter")]
    public AbilityModifierAction[] OnFieldEnter { get; set; } = [];

    [JsonPropertyName("onExit")]
    public AbilityModifierAction[] OnExit { get; set; } = [];

    [JsonPropertyName("onAttach")]
    public AbilityModifierAction[] OnAttach { get; set; } = [];

    [JsonPropertyName("onDetach")]
    public AbilityModifierAction[] OnDetach { get; set; } = [];

    [JsonPropertyName("onAvatarIn")]
    public AbilityModifierAction[] OnAvatarIn { get; set; } = [];

    [JsonPropertyName("onAvatarOut")]
    public AbilityModifierAction[] OnAvatarOut { get; set; } = [];

    [JsonPropertyName("onTriggerAvatarRay")]
    public AbilityModifierAction[] OnTriggerAvatarRay { get; set; } = [];

    [JsonPropertyName("onVehicleIn")]
    public AbilityModifierAction[] OnVehicleIn { get; set; } = [];

    [JsonPropertyName("onVehicleOut")]
    public AbilityModifierAction[] OnVehicleOut { get; set; } = [];

    [JsonPropertyName("abilityMixins")]
    public AbilityMixinData[] AbilityMixins { get; set; } = [];

    [JsonIgnore]
    public Dictionary<int, AbilityModifierAction> LocalIdToAction { get; } = new();

    [JsonIgnore]
    public Dictionary<int, AbilityMixinData> LocalIdToMixin { get; } = new();

    private bool _initialized = false;

    public void Initialize()
    {
        if (_initialized) return;
        _initialized = true;

        InitializeMixins();
        InitializeModifiers();
        InitializeActions();
    }

    private void InitializeMixins()
    {
        if (AbilityMixins == null) return;

        var generator = new AbilityLocalIdGenerator(AbilityLocalIdGenerator.ConfigAbilitySubContainerType.MIXIN);
        generator.ModifierIndex = 0;
        generator.ConfigIndex = 0;

        generator.InitializeMixinsLocalIds(AbilityMixins, LocalIdToMixin);
    }

    private void InitializeModifiers()
    {
        if (Modifiers == null)
        {
            Modifiers = new Dictionary<string, AbilityModifier>();
            return;
        }

        var sortedModifiers = Modifiers.OrderBy(kv => kv.Key).Select(kv => kv.Value).ToList();

        var modifierIndex = 0;
        foreach (var abilityModifier in sortedModifiers)
        {
            long configIndex = 0L;
            InitializeActionSubCategory(modifierIndex, configIndex++, abilityModifier.OnAdded, LocalIdToAction);
            InitializeActionSubCategory(modifierIndex, configIndex++, abilityModifier.OnRemoved, LocalIdToAction);
            InitializeActionSubCategory(modifierIndex, configIndex++, abilityModifier.OnBeingHit, LocalIdToAction);
            InitializeActionSubCategory(modifierIndex, configIndex++, abilityModifier.OnAttackLanded, LocalIdToAction);
            InitializeActionSubCategory(modifierIndex, configIndex++, abilityModifier.OnHittingOther, LocalIdToAction);
            InitializeActionSubCategory(modifierIndex, configIndex++, abilityModifier.OnThinkInterval, LocalIdToAction);
            InitializeActionSubCategory(modifierIndex, configIndex++, abilityModifier.OnKill, LocalIdToAction);
            InitializeActionSubCategory(modifierIndex, configIndex++, abilityModifier.OnCrash, LocalIdToAction);
            InitializeActionSubCategory(modifierIndex, configIndex++, abilityModifier.OnAvatarIn, LocalIdToAction);
            InitializeActionSubCategory(modifierIndex, configIndex++, abilityModifier.OnAvatarOut, LocalIdToAction);
            InitializeActionSubCategory(modifierIndex, configIndex++, abilityModifier.OnReconnect, LocalIdToAction);
            InitializeActionSubCategory(modifierIndex, configIndex++, abilityModifier.OnChangeAuthority, LocalIdToAction);
            InitializeActionSubCategory(modifierIndex, configIndex++, abilityModifier.OnVehicleIn, LocalIdToAction);
            InitializeActionSubCategory(modifierIndex, configIndex++, abilityModifier.OnVehicleOut, LocalIdToAction);
            InitializeActionSubCategory(modifierIndex, configIndex++, abilityModifier.OnZoneEnter, LocalIdToAction);
            InitializeActionSubCategory(modifierIndex, configIndex++, abilityModifier.OnZoneExit, LocalIdToAction);
            InitializeActionSubCategory(modifierIndex, configIndex++, abilityModifier.OnHeal, LocalIdToAction);
            InitializeActionSubCategory(modifierIndex, configIndex++, abilityModifier.OnBeingHealed, LocalIdToAction);

            if (abilityModifier.ModifierMixins != null && abilityModifier.ModifierMixins.Length > 0)
            {
                var mixinGenerator = new AbilityLocalIdGenerator(AbilityLocalIdGenerator.ConfigAbilitySubContainerType.MODIFIER_MIXIN);
                mixinGenerator.ModifierIndex = modifierIndex;
                mixinGenerator.ConfigIndex = 0;

                mixinGenerator.InitializeMixinsLocalIds(abilityModifier.ModifierMixins, LocalIdToMixin);
            }

            modifierIndex++;
        }
    }

    private void InitializeActionSubCategory(
        long modifierIndex,
        long configIndex,
        AbilityModifierAction[] actions,
        Dictionary<int, AbilityModifierAction> localIdToAction)
    {
        if (actions == null) return;

        var generator = new AbilityLocalIdGenerator(AbilityLocalIdGenerator.ConfigAbilitySubContainerType.MODIFIER_ACTION);
        generator.ModifierIndex = modifierIndex;
        generator.ConfigIndex = configIndex;

        generator.InitializeActionLocalIds(actions, localIdToAction);
    }

    private void InitializeActions()
    {
        var generator = new AbilityLocalIdGenerator(AbilityLocalIdGenerator.ConfigAbilitySubContainerType.ACTION);
        generator.ConfigIndex = 0;

        generator.InitializeActionLocalIds(OnAdded, LocalIdToAction);
        generator.ConfigIndex++;
        generator.InitializeActionLocalIds(OnRemoved, LocalIdToAction);
        generator.ConfigIndex++;
        generator.InitializeActionLocalIds(OnAbilityStart, LocalIdToAction);
        generator.ConfigIndex++;
        generator.InitializeActionLocalIds(OnKill, LocalIdToAction);
        generator.ConfigIndex++;
        generator.InitializeActionLocalIds(OnFieldEnter, LocalIdToAction);
        generator.ConfigIndex++;
        generator.InitializeActionLocalIds(OnExit, LocalIdToAction);
        generator.ConfigIndex++;
        generator.InitializeActionLocalIds(OnAttach, LocalIdToAction);
        generator.ConfigIndex++;
        generator.InitializeActionLocalIds(OnDetach, LocalIdToAction);
        generator.ConfigIndex++;
        generator.InitializeActionLocalIds(OnAvatarIn, LocalIdToAction);
        generator.ConfigIndex++;
        generator.InitializeActionLocalIds(OnAvatarOut, LocalIdToAction);
        generator.ConfigIndex++;
        generator.InitializeActionLocalIds(OnTriggerAvatarRay, LocalIdToAction);
        generator.ConfigIndex++;
        generator.InitializeActionLocalIds(OnVehicleIn, LocalIdToAction);
        generator.ConfigIndex++;
        generator.InitializeActionLocalIds(OnVehicleOut, LocalIdToAction);
    }
}
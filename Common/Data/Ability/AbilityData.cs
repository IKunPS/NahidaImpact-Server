using System.Text.Json.Serialization;
using System.Collections.Generic;

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

        // TODO: Implement local ID generation for mixins
        // For now, we'll leave it empty
    }

    private void InitializeModifiers()
    {
        if (Modifiers == null)
        {
            Modifiers = new Dictionary<string, AbilityModifier>();
            return;
        }

        // TODO: Implement local ID generation for modifier actions
    }

    private void InitializeActions()
    {
        // TODO: Implement local ID generation for actions
    }
}
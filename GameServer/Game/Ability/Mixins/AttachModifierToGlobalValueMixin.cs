using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;

namespace NahidaImpact.GameServer.Game.Ability.Mixins;

[AbilityMixin(AbilityMixinData.MixinType.AttachModifierToGlobalValueMixin)]
public class AttachModifierToGlobalValueMixin : AbilityMixinHandler
{
    // hk4e AttachModifierToGlobalValueMixin — attaches modifiers to target based on global value thresholds
    public override Task<bool> Execute(Ability ability, AbilityMixinData mixinData, ByteString abilityData, BaseEntity target)
    {
        if (string.IsNullOrEmpty(mixinData.GlobalValueKey)) return Task.FromResult(false);

        if (!target.GlobalAbilityValues.TryGetValue(mixinData.GlobalValueKey, out var globalValue))
            globalValue = mixinData.DefaultGlobalValueOnCreate;

        string? modifierName = null;
        if (mixinData.RatioSteps != null && mixinData.ModifierNameSteps != null)
        {
            for (int i = 0; i < mixinData.RatioSteps.Count && i < mixinData.ModifierNameSteps.Count; i++)
            {
                if (globalValue >= mixinData.RatioSteps[i])
                    modifierName = mixinData.ModifierNameSteps[i];
            }
        }

        if (string.IsNullOrEmpty(modifierName))
        {
            var names = mixinData.ModifierNames;
            if (names.Count > 0) modifierName = names[0];
        }

        if (string.IsNullOrEmpty(modifierName)) return Task.FromResult(false);
        if (!ability.Data.Modifiers.TryGetValue(modifierName, out var modData)) return Task.FromResult(false);

        var modCtrl = new AbilityModifierController(ability, ability.Data, modData)
        {
            OwnerEntity = target,
            ApplyEntityId = target.Id
        };
        ability.Modifiers[modifierName] = modCtrl;

        return Task.FromResult(true);
    }
}

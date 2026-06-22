using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace NahidaImpact.GameServer.Game.Ability.Mixins;

[AbilityMixin(AbilityMixinData.MixinType.AttachModifierToSelfGlobalValueMixin)]
public class AttachModifierToSelfGlobalValueMixin : AbilityMixinHandler
{
    public override Task<bool> Execute(Ability ability, AbilityMixinData mixinData, ByteString abilityData, BaseEntity target)
    {
        var owner = ability.Owner;
        if (owner == null || string.IsNullOrEmpty(mixinData.GlobalValueKey))
            return Task.FromResult(false);

        if (!owner.GlobalAbilityValues.TryGetValue(mixinData.GlobalValueKey, out var globalValue))
            return Task.FromResult(false);

        // Find matching modifier by comparing against ratio steps
        string? modifierName = null;
        if (mixinData.RatioSteps != null && mixinData.ModifierNameSteps != null)
        {
            for (int i = 0; i < mixinData.RatioSteps.Count && i < mixinData.ModifierNameSteps.Count; i++)
            {
                if (globalValue >= mixinData.RatioSteps[i])
                {
                    modifierName = mixinData.ModifierNameSteps[i];
                }
            }
        }

        if (modifierName == null && mixinData.ModifierName != null)
        {
            var names = mixinData.ModifierNames;
            modifierName = names.FirstOrDefault();
        }

        if (string.IsNullOrEmpty(modifierName)) return Task.FromResult(false);

        if (!ability.Data.Modifiers.TryGetValue(modifierName, out var modifierData))
            return Task.FromResult(false);

        var modifier = new AbilityModifierController(ability, ability.Data, modifierData);
        ability.Modifiers[modifierName] = modifier;

        return Task.FromResult(true);
    }
}

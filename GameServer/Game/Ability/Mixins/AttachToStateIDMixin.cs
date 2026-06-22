using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using System.Threading.Tasks;

namespace NahidaImpact.GameServer.Game.Ability.Mixins;

[AbilityMixin(AbilityMixinData.MixinType.AttachToStateIDMixin)]
public class AttachToStateIDMixin : AbilityMixinHandler
{
    public override Task<bool> Execute(Ability ability, AbilityMixinData mixinData, ByteString abilityData, BaseEntity target)
    {
        // Attach modifier to each stateID in the mixin data
        if (mixinData.StateIDs == null || mixinData.StateIDs.Count == 0)
            return Task.FromResult(false);

        var modifierNames = mixinData.ModifierNames;
        if (modifierNames.Count == 0) return Task.FromResult(false);

        foreach (var stateId in mixinData.StateIDs)
        {
            foreach (var modifierName in modifierNames)
            {
                if (ability.Data.Modifiers.TryGetValue(modifierName, out var modifierData))
                {
                    var modifier = new AbilityModifierController(ability, ability.Data, modifierData);
                    ability.Modifiers[modifierName] = modifier;
                    // TODO: Associate modifier with stateID
                }
            }
        }

        return Task.FromResult(true);
    }
}

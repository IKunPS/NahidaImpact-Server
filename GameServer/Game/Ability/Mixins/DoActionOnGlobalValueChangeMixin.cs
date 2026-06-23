using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;

namespace NahidaImpact.GameServer.Game.Ability.Mixins;

[AbilityMixin(AbilityMixinData.MixinType.DoActionOnGlobalValueChangeMixin)]
public class DoActionOnGlobalValueChangeMixin : AbilityMixinHandler
{
    // hk4e DoActionOnGlobalValueChangeMixin — fires actions when global values cross thresholds
    public override Task<bool> Execute(Ability ability, AbilityMixinData mixinData, ByteString abilityData, BaseEntity target)
    {
        if (string.IsNullOrEmpty(mixinData.GlobalValueKey)) return Task.FromResult(false);

        if (!target.GlobalAbilityValues.TryGetValue(mixinData.GlobalValueKey, out var value))
            return Task.FromResult(false);

        // Apply events from modifier lifecycle that react to this value change
        foreach (var modCtrl in ability.Modifiers.Values)
        {
            if (modCtrl.ModifierData.OnThinkInterval != null)
                foreach (var action in modCtrl.ModifierData.OnThinkInterval)
                    ability.Manager.ExecuteAction(ability, action, abilityData, target);
        }

        return Task.FromResult(true);
    }
}

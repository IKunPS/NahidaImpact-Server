using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;

namespace NahidaImpact.GameServer.Game.Ability.Mixins;

[AbilityMixin(AbilityMixinData.MixinType.ModifyDamageMixin)]
public class ModifyDamageMixin : AbilityMixinHandler
{
    // hk4e AbilityModifyDamageMixin — caches damage modifier on the entity for Damage() to apply
    public override Task<bool> Execute(Ability ability, AbilityMixinData mixinData, ByteString abilityData, BaseEntity target)
    {
        if (target is not EntityAvatar avatar) return Task.FromResult(false);

        // Store damage ratio in global ability values for the damage pipeline to check
        avatar.GlobalAbilityValues["_ModifyDamage_Ratio"] = mixinData.Ratio;
        return Task.FromResult(true);
    }
}

using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;

namespace NahidaImpact.GameServer.Game.Ability.Mixins;

[AbilityMixin(AbilityMixinData.MixinType.ShieldBarMixin)]
public class ShieldBarMixin : AbilityMixinHandler
{
    // hk4e AbilityShieldBarMixin — tracks shield state via global ability value for UI display
    public override Task<bool> Execute(Ability ability, AbilityMixinData mixinData, ByteString abilityData, BaseEntity target)
    {
        // Shield value is tracked via modifier properties and global ability values
        // The client reads AbilitySyncStateInfo for shield bar rendering
        target.GlobalAbilityValues["_ShieldBar_Ratio"] = mixinData.Ratio;
        return Task.FromResult(true);
    }
}

using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using System.Threading.Tasks;

namespace NahidaImpact.GameServer.Game.Ability.Mixins;

[AbilityMixin(AbilityMixinData.MixinType.ReviveElemEnergyMixin)]
public class ReviveElemEnergyMixin : AbilityMixinHandler
{
    public override Task<bool> Execute(Ability ability, AbilityMixinData mixinData, ByteString abilityData, BaseEntity target)
    {
        float ratio = mixinData.Ratio;

        // TODO: Add energy to target avatar based on current energy type
        // For now, placeholder

        return Task.FromResult(true);
    }
}

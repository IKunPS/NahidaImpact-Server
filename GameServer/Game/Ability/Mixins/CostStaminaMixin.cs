using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using System;
using System.Threading.Tasks;

namespace NahidaImpact.GameServer.Game.Ability.Mixins;

[AbilityMixin(AbilityMixinData.MixinType.CostStaminaMixin)]
public class CostStaminaMixin : AbilityMixinHandler
{
    private DateTime _lastCostTime = DateTime.MinValue;
    private static readonly TimeSpan Cooldown = TimeSpan.FromMilliseconds(500);

    public override Task<bool> Execute(Ability ability, AbilityMixinData mixinData, ByteString abilityData, BaseEntity target)
    {
        var now = DateTime.UtcNow;
        if (now - _lastCostTime < Cooldown)
            return Task.FromResult(true);

        _lastCostTime = now;

        // TODO: Consume stamina from player via StaminaManager
        float costDelta = mixinData.CostStaminaDelta;
        // abilityManager?.GetPlayer()?.StaminaManager?.ConsumeStamina(costDelta);

        return Task.FromResult(true);
    }
}

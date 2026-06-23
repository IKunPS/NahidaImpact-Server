using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.Prop;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("ReviveElemEnergy")]
public class ActionReviveElemEnergy : AbilityActionHandler
{
    // hk4e ReviveElemEnergyImpl — restores elemental energy to caster
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        var caster = ability.Caster ?? ability.Owner;
        if (caster == null) return Task.FromResult(false);

        var ratio = action.Ratio > 0 ? action.Ratio : 1f;
        var amount = action.BaseEnergy * ratio;
        if (amount <= 0) return Task.FromResult(true);

        caster.AddSpecialEnergy(amount);
        return Task.FromResult(true);
    }
}

using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.Prop;
using System.Threading.Tasks;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("SetOverrideMapValue")]
public class ActionSetOverrideMapValue : AbilityActionHandler
{
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        if (string.IsNullOrEmpty(action.OverrideMapKey)) return Task.FromResult(false);

        var owner = ability.Owner;
        if (owner == null) return Task.FromResult(false);

        float value = action.Amount;

        // Add fight property contributions
        value += action.AmountByCasterMaxHPRatio * owner.GetFightProperty(FightProp.FIGHT_PROP_MAX_HP);
        value += action.AmountByCasterAttackRatio * owner.GetFightProperty(FightProp.FIGHT_PROP_CUR_ATTACK);

        ability.AbilitySpecials[action.OverrideMapKey] = value * action.Ratio;

        return Task.FromResult(true);
    }
}

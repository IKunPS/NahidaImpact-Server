using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.Prop;
using System.Threading.Tasks;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("GetHPPaidDebts")]
public class ActionGetHPPaidDebts : AbilityActionHandler
{
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        float paidDebts = target.GetFightProperty(FightProp.FIGHT_PROP_CUR_HP_PAID_DEBTS);

        if (!string.IsNullOrEmpty(action.OverrideMapKey))
        {
            ability.AbilitySpecials[action.OverrideMapKey] = paidDebts;
        }

        // TODO: Broadcast fight property updates

        return Task.FromResult(true);
    }
}

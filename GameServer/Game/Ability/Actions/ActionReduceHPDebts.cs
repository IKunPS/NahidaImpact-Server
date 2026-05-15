using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.Prop;
using System.Threading.Tasks;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("ReduceHPDebts")]
public class ActionReduceHPDebts : AbilityActionHandler
{
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        float curDebts = target.GetFightProperty(FightProp.FIGHT_PROP_CUR_HP_DEBTS);
        float maxHp = target.GetFightProperty(FightProp.FIGHT_PROP_MAX_HP);

        float reduceAmount = action.Ratio * curDebts;
        reduceAmount = System.Math.Min(reduceAmount, curDebts);

        target.SetFightProperty((int)FightProp.FIGHT_PROP_CUR_HP_DEBTS, curDebts - reduceAmount);

        // TODO: Broadcast fight prop update and change reason notify

        return Task.FromResult(true);
    }
}

using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.GameServer.Server.Packet.Send.Entity;
using NahidaImpact.Prop;
using NahidaImpact.Proto;
using System.Threading.Tasks;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("AddHPDebts")]
public class ActionAddHPDebts : AbilityActionHandler
{
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        var owner = ability.Owner;
        if (owner == null) return Task.FromResult(false);

        float maxHp = target.GetFightProperty(FightProp.FIGHT_PROP_MAX_HP);
        float curDebts = target.GetFightProperty(FightProp.FIGHT_PROP_CUR_HP_DEBTS);

        float amountToAdd = action.Amount;
        amountToAdd += action.AmountByCasterMaxHPRatio * owner.GetFightProperty(FightProp.FIGHT_PROP_MAX_HP);
        amountToAdd += action.AmountByCasterAttackRatio * owner.GetFightProperty(FightProp.FIGHT_PROP_CUR_ATTACK);
        amountToAdd += action.AmountByTargetCurrentHPRatio * target.GetFightProperty(FightProp.FIGHT_PROP_CUR_HP);
        amountToAdd += action.AmountByTargetMaxHPRatio * maxHp;

        float newDebts = System.Math.Clamp(curDebts + amountToAdd, 0, 2 * maxHp);
        target.SetFightProperty((int)FightProp.FIGHT_PROP_CUR_HP_DEBTS, newDebts);

        target.Scene?.BroadcastPacket(new PacketEntityFightPropUpdateNotify(target, FightProp.FIGHT_PROP_CUR_HP_DEBTS));
        target.Scene?.BroadcastPacket(new PacketEntityFightPropChangeReasonNotify(
            target, FightProp.FIGHT_PROP_CUR_HP_DEBTS, amountToAdd,
            PropChangeReason.Ability, ChangeHpDebtsReason.AddAbility));

        return Task.FromResult(true);
    }
}
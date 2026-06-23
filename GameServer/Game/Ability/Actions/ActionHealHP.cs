using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.Prop;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("HealHP")]
public class ActionHealHP : AbilityActionHandler
{
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        var owner = ability.Owner;

        // TODO: client gadget owner resolution
        if (owner == null) return Task.FromResult(false);

        var amount = action.Amount;
        amount += action.AmountByCasterMaxHPRatio * owner.GetFightProperty(FightProp.FIGHT_PROP_MAX_HP);
        amount += action.AmountByCasterAttackRatio * owner.GetFightProperty(FightProp.FIGHT_PROP_CUR_ATTACK);
        amount += action.AmountByCasterCurrentHPRatio * owner.GetFightProperty(FightProp.FIGHT_PROP_CUR_HP);

        var abilityRatio = 1f;
        if (!action.IgnoreAbilityProperty)
        {
            abilityRatio += target.GetFightProperty(FightProp.FIGHT_PROP_HEAL_ADD)
                + target.GetFightProperty(FightProp.FIGHT_PROP_HEALED_ADD);
        }

        amount += action.AmountByTargetCurrentHPRatio * target.GetFightProperty(FightProp.FIGHT_PROP_CUR_HP);
        amount += action.AmountByTargetMaxHPRatio * target.GetFightProperty(FightProp.FIGHT_PROP_MAX_HP);

        target.Heal(amount * abilityRatio * action.HealRatio, action.MuteHealEffect);
        return Task.FromResult(true);
    }
}

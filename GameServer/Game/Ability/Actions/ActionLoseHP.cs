using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.Prop;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("LoseHP")]
public class ActionLoseHP : AbilityActionHandler
{
    // hk4e LoseHPImpl — scales damage by caster/target stats, supports limbo and lethal guard
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        var owner = ability.Owner;
        // TODO: client gadget owner resolution with AbilityInvulnerable check
        if (owner == null) return Task.FromResult(false);

        if (action.EnableLockHP && target.LockHP) return Task.FromResult(true);
        if (action.DisableWhenLoading) return Task.FromResult(true);

        var amount = action.Amount;
        amount += action.AmountByCasterMaxHPRatio * owner.GetFightProperty(FightProp.FIGHT_PROP_MAX_HP);
        amount += action.AmountByCasterAttackRatio * owner.GetFightProperty(FightProp.FIGHT_PROP_CUR_ATTACK);
        amount += action.AmountByCasterCurrentHPRatio * owner.GetFightProperty(FightProp.FIGHT_PROP_CUR_HP);

        var curHp = target.GetFightProperty(FightProp.FIGHT_PROP_CUR_HP);
        var maxHp = target.GetFightProperty(FightProp.FIGHT_PROP_MAX_HP);
        amount += action.AmountByTargetCurrentHPRatio * curHp;
        amount += action.AmountByTargetMaxHPRatio * maxHp;

        // hk4e limbo guard — prevent 1-shot below threshold
        if (action.LimboByTargetMaxHPRatio > 1.192093e-07f)
            amount = Math.Min(Math.Max(curHp - Math.Max(action.LimboByTargetMaxHPRatio * maxHp, 1f), 0f), amount);

        // Non-lethal guard — don't kill
        if (curHp < amount + 0.01f && !action.Lethal) amount = 0;
        // Floor damage
        if (amount == 0) amount = 0.47f * maxHp;

        target.Damage(amount);
        return Task.FromResult(true);
    }
}

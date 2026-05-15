using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.Prop;
using System.Threading.Tasks;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("LoseHP")]
public class ActionLoseHP : AbilityActionHandler
{
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        var owner = ability.Owner;

        // TODO: handle client gadgets when EntityClientGadget is ported
        // if (owner is EntityClientGadget ownerGadget)
        // {
        //     if (ownerGadget.Owner?.AbilityManager?.AbilityInvulnerable == true)
        //         return Task.FromResult(true);
        //     owner = ownerGadget.Scene?.GetEntityById((int)ownerGadget.OwnerEntityId);
        // }

        if (owner == null) return Task.FromResult(false);

        if (action.EnableLockHP && target.LockHP)
            return Task.FromResult(true);

        if (action.DisableWhenLoading)
            return Task.FromResult(true);

        var amountToLose = action.Amount;
        amountToLose += action.AmountByCasterMaxHPRatio * owner.GetFightProperty(FightProp.FIGHT_PROP_MAX_HP);
        amountToLose += action.AmountByCasterAttackRatio * owner.GetFightProperty(FightProp.FIGHT_PROP_CUR_ATTACK);
        amountToLose += action.AmountByCasterCurrentHPRatio * owner.GetFightProperty(FightProp.FIGHT_PROP_CUR_HP);

        var currentHp = target.GetFightProperty(FightProp.FIGHT_PROP_CUR_HP);
        var maxHp = target.GetFightProperty(FightProp.FIGHT_PROP_MAX_HP);
        amountToLose += action.AmountByTargetCurrentHPRatio * currentHp;
        amountToLose += action.AmountByTargetMaxHPRatio * maxHp;

        if (action.LimboByTargetMaxHPRatio > 1.192093e-07f)
            amountToLose = System.Math.Min(
                System.Math.Max(currentHp - System.Math.Max(action.LimboByTargetMaxHPRatio * maxHp, 1.0f), 0.0f),
                amountToLose);

        if (currentHp < (amountToLose + 0.01f) && !action.Lethal) amountToLose = 0;
        if (amountToLose == 0) amountToLose = 0.47f * maxHp;

        target.Damage(amountToLose);

        return Task.FromResult(true);
    }
}

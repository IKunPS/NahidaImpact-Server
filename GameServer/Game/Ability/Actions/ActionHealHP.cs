using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.Prop;
using System.Threading.Tasks;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("HealHP")]
public class ActionHealHP : AbilityActionHandler
{
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        var owner = ability.Owner;

        // TODO: handle client gadgets when EntityClientGadget is ported
        // if (owner is EntityClientGadget ownerGadget)
        //     owner = ownerGadget.Scene?.GetEntityById((int)ownerGadget.OwnerEntityId);

        if (owner == null) return Task.FromResult(false);

        // Collect all properties (fight props + ability specials)
        float amountToRegenerate = action.Amount;

        amountToRegenerate += action.AmountByCasterMaxHPRatio * owner.GetFightProperty(FightProp.FIGHT_PROP_MAX_HP);
        amountToRegenerate += action.AmountByCasterAttackRatio * owner.GetFightProperty(FightProp.FIGHT_PROP_CUR_ATTACK);
        amountToRegenerate += action.AmountByCasterCurrentHPRatio * owner.GetFightProperty(FightProp.FIGHT_PROP_CUR_HP);

        float abilityRatio = 1.0f;
        if (!action.IgnoreAbilityProperty)
        {
            abilityRatio += target.GetFightProperty(FightProp.FIGHT_PROP_HEAL_ADD)
                + target.GetFightProperty(FightProp.FIGHT_PROP_HEALED_ADD);
        }

        amountToRegenerate += action.AmountByTargetCurrentHPRatio * target.GetFightProperty(FightProp.FIGHT_PROP_CUR_HP);
        amountToRegenerate += action.AmountByTargetMaxHPRatio * target.GetFightProperty(FightProp.FIGHT_PROP_MAX_HP);

        target.Heal(amountToRegenerate * abilityRatio * action.HealRatio, action.MuteHealEffect);

        return Task.FromResult(true);
    }
}

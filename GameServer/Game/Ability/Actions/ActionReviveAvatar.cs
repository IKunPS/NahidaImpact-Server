using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.Prop;
using System.Linq;
using System.Threading.Tasks;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("ReviveAvatar")]
public class ActionReviveAvatar : AbilityActionHandler
{
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        // Resurrect dead team members
        var team = ability.PlayerOwner?.TeamManager?.GetActiveTeam();
        if (team == null) return Task.FromResult(false);

        foreach (var avatar in team)
        {
            if (!avatar.IsAlive())
            {
                float maxHp = avatar.GetFightProperty(FightProp.FIGHT_PROP_MAX_HP);
                float healAmount = action.AmountByTargetMaxHPRatio * maxHp;
                avatar.Heal(System.Math.Max(healAmount, 1f), action.MuteHealEffect);
                avatar.IsDead = false;
                avatar.LifeState = 1;
                // TODO: Broadcast PacketAvatarLifeStateChangeNotify
            }
        }

        return Task.FromResult(true);
    }
}

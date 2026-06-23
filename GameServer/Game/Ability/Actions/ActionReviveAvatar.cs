using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.GameServer.Server.Packet.Send.Avatar;
using NahidaImpact.Prop;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("ReviveAvatar")]
public class ActionReviveAvatar : AbilityActionHandler
{
    // hk4e ReviveAvatarImpl — resurrects dead team members with HP restore and life state broadcast
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        var player = ability.PlayerOwner;
        var team = player?.TeamManager?.GetActiveTeam();
        if (team == null) return Task.FromResult(false);

        foreach (var avatar in team)
        {
            if (avatar.IsAlive()) continue;

            var maxHp = avatar.GetFightProperty(FightProp.FIGHT_PROP_MAX_HP);
            var healAmount = Math.Max(action.AmountByTargetMaxHPRatio * maxHp, 1f);
            avatar.Heal(healAmount, action.MuteHealEffect);
            avatar.IsDead = false;
            avatar.LifeState = 1;

            if (player != null)
                _ = player.SendPacket(new PacketAvatarLifeStateChangeNotify(avatar.AvatarInfo));
        }

        return Task.FromResult(true);
    }
}

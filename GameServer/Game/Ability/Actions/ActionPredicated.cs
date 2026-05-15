using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.Prop;
using System;
using System.Threading.Tasks;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("Predicated")]
public class ActionPredicated : AbilityActionHandler
{
    private static readonly float[] DamageMultipliers = { 0.024f, 0.016f, 0.024f, 0.016f, 0.024f, 0.016f, 0.024f, 0.036f };
    private static readonly Random Random = new();

    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        // Furina-specific HP consumption logic
        int avatarId = (int)(target is EntityAvatar avatar ? avatar.AvatarInfo.AvatarId : 0);
        if (avatarId != 10000089) return Task.FromResult(true);

        float multiplier = DamageMultipliers[Random.Next(DamageMultipliers.Length)];

        var team = ability.PlayerOwner?.TeamManager?.GetActiveTeam();
        if (team == null) return Task.FromResult(false);

        foreach (var teamMember in team)
        {
            float curHP = teamMember.GetFightProperty(FightProp.FIGHT_PROP_CUR_HP);
            float maxHP = teamMember.GetFightProperty(FightProp.FIGHT_PROP_MAX_HP);
            float consumeHP = multiplier * maxHP;
            int memberId = (int)teamMember.AvatarInfo.AvatarId;
            bool isFurina = memberId == 10000089;

            if ((isFurina && curHP > 0.55f * maxHP) || (!isFurina && curHP > 0.5f * maxHP))
            {
                teamMember.Damage(consumeHP);
            }
        }

        return Task.FromResult(true);
    }
}

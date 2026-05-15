using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.Prop;
using System;
using System.Threading.Tasks;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("SetGlobalValue")]
public class ActionSetGlobalValue : AbilityActionHandler
{
    private static readonly float[] DamageMultipliers = { 0.024f, 0.016f, 0.024f, 0.016f, 0.024f, 0.016f, 0.024f, 0.036f };
    private static readonly Random Random = new();

    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        var owner = ability.Owner;

        string valueKey = action.Key;
        float value = action.Ratio; // Simple float ratio

        target.GlobalAbilityValues[valueKey] = value;
        target.OnAbilityValueUpdate();

        // Furina Arkhe HP consumption special case
        if ("_ABILITY_ArkheGrade_Attack_CD".Equals(valueKey))
        {
            var team = ability.PlayerOwner?.TeamManager?.GetActiveTeam();
            if (team != null)
            {
                float multiplier = DamageMultipliers[Random.Next(DamageMultipliers.Length)];
                foreach (var teamMember in team)
                {
                    float curHP = teamMember.GetFightProperty(FightProp.FIGHT_PROP_CUR_HP);
                    float maxHP = teamMember.GetFightProperty(FightProp.FIGHT_PROP_MAX_HP);
                    float consumeHP = multiplier * maxHP;
                    int avatarId = (int)teamMember.AvatarInfo.AvatarId;
                    bool isFurina = avatarId == 10000089;

                    if ((isFurina && curHP > 0.55f * maxHP) || (!isFurina && curHP > 0.5f * maxHP))
                    {
                        teamMember.Damage(consumeHP);
                    }
                }
            }
        }

        return Task.FromResult(true);
    }
}

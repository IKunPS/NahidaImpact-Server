using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.GameServer.Game.Player;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("AvatarSkillStart")]
public class ActionAvatarSkillStart : AbilityActionHandler
{
    // hk4e AvatarSkillStartImpl — marks skill start for burst invulnerability tracking and quest triggers
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        if (ability.PlayerOwner != null)
            ability.Manager.OnSkillStart(ability.PlayerOwner, action.SkillID, (int)target.Id);

        return Task.FromResult(true);
    }
}

using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using System.Threading.Tasks;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("AvatarSkillStart")]
public class ActionAvatarSkillStart : AbilityActionHandler
{
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        // TODO: Trigger quest content for allowed skill IDs when quest system is ready
        // TODO: Handle stamina cost via costStaminaRatio

        return Task.FromResult(true);
    }
}

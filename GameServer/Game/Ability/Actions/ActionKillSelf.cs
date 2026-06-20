using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using System.Threading.Tasks;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("KillSelf")]
public class ActionKillSelf : AbilityActionHandler
{
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        // Kill the target entity
        target.Damage(float.MaxValue);

        // TODO: Handle otherTargets with configID when supported

        return Task.FromResult(true);
    }
}

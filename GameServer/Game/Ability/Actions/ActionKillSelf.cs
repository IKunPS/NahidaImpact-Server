using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("KillSelf")]
public class ActionKillSelf : AbilityActionHandler
{
    // hk4e KillSelfImpl — kills the target entity and optionally other entities matching configId
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        target.Damage(float.MaxValue);

        if (action.ConfigID > 0 && target.Scene != null)
        {
            var matching = target.Scene.GetEntitiesByConfigId(action.ConfigID);
            foreach (var entity in matching)
                if (entity != target)
                    entity.Damage(float.MaxValue);
        }

        return Task.FromResult(true);
    }
}

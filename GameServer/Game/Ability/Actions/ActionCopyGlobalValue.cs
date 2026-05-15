using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using System.Threading.Tasks;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("CopyGlobalValue")]
public class ActionCopyGlobalValue : AbilityActionHandler
{
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        var srcTarget = GetTarget(ability, target, action.SrcTarget);
        var dstTarget = GetTarget(ability, target, action.DstTarget);

        if (srcTarget == null || dstTarget == null) return Task.FromResult(false);

        if (srcTarget.GlobalAbilityValues.TryGetValue(action.SrcKey, out var value))
        {
            dstTarget.GlobalAbilityValues[action.DstKey] = value;
            dstTarget.OnAbilityValueUpdate();
        }

        return Task.FromResult(true);
    }
}

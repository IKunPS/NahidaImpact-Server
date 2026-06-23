using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("ServerLuaCall")]
public class ActionServerLuaCall : AbilityActionHandler
{
    // hk4e ServerLuaCallImpl — dispatches Lua calls to entity scripts based on LuaCallType
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        target.OnClientExecuteRequest(
            (int)action.Param1,
            (int)action.Param2,
            (int)action.Param3);

        return Task.FromResult(true);
    }
}

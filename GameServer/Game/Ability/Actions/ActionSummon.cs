using Google.Protobuf;
using NahidaImpact.Data;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using System.Threading.Tasks;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("Summon")]
public class ActionSummon : AbilityActionHandler
{
    // hk4e SummonImpl — creates a monster entity from config at caster position
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        // TODO: parse AbilityActionSummon proto for pos/rot/monsterId/summonTag
        var owner = ability.Owner;
        if (owner?.Scene == null || action.MonsterID <= 0) return Task.FromResult(false);

        if (!GameData.MonsterData.TryGetValue(action.MonsterID, out var monsterData))
            return Task.FromResult(false);

        var pos = owner.Position.Clone();
        var rot = owner.Rotation.Clone();

        var monster = new EntityMonster(owner.Scene, monsterData, pos, rot, 1);
        // TODO: summonedTag, ownerEntityId
        owner.Scene.AddEntity(monster);

        return Task.FromResult(true);
    }
}

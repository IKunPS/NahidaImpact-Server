using Google.Protobuf;
using NahidaImpact.Data;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("Summon")]
public class ActionSummon : AbilityActionHandler
{
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        // TODO: Parse AbilityActionSummon proto when available
        var owner = ability.Owner;
        if (owner?.Scene == null || action.MonsterID <= 0) return Task.FromResult(false);

        if (!GameData.MonsterData.TryGetValue(action.MonsterID, out var monsterData))
            return Task.FromResult(false);

        // TODO: Parse position/rotation from proto
        var pos = owner.Position.Clone();
        var rot = owner.Rotation.Clone();

        var monster = new EntityMonster(owner.Scene, monsterData, pos, rot, 1);

        // TODO: Set summon tags (summonedTag, ownerEntityId)
        owner.Scene.AddEntity(monster);

        return Task.FromResult(true);
    }
}

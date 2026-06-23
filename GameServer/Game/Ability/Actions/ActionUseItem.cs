using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("UseItem")]
public class ActionUseItem : AbilityActionHandler
{
    // hk4e UseItemImpl — ability consumes an item from the player's inventory
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        var player = ability.PlayerOwner;
        if (player == null) return Task.FromResult(false);

        var itemId = action.ConfigID > 0 ? action.ConfigID : action.MonsterID;
        if (itemId <= 0) return Task.FromResult(false);

        var item = player.InventoryManager.GetFirstItem(itemId);
        if (item == null) return Task.FromResult(false);

        var count = action.Amount > 0 ? (uint)action.Amount : 1;
        player.InventoryManager.UseItem(item.Guid, count, 0, 0);
        return Task.FromResult(true);
    }
}

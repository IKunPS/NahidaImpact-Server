using NahidaImpact.Database.Inventory;

namespace NahidaImpact.GameServer.Game.Inventory;

public interface IInventoryTab
{
    ItemData? GetItemById(int id);
    void OnAddItem(ItemData item);
    void OnRemoveItem(ItemData item);
    int Size { get; }
    int MaxCapacity { get; }
    int GetItemCountById(int itemId);
}

using System.Collections.Generic;
using NahidaImpact.Database.Inventory;

namespace NahidaImpact.GameServer.Game.Inventory;

/// <summary>Tab for stackable items (materials, furniture) — keyed by item ID for fast lookup.</summary>
public class MaterialInventoryTab : IInventoryTab
{
    private readonly Dictionary<int, ItemData> _items = [];
    private readonly int _maxCapacity;

    public MaterialInventoryTab(int maxCapacity)
    {
        _maxCapacity = maxCapacity;
    }

    public int Size => _items.Count;
    public int MaxCapacity => _maxCapacity;

    public ItemData? GetItemById(int id) => _items.GetValueOrDefault(id);

    public void OnAddItem(ItemData item) => _items[item.ItemId] = item;

    public void OnRemoveItem(ItemData item) => _items.Remove(item.ItemId);

    public int GetItemCountById(int itemId)
    {
        var item = GetItemById(itemId);
        return item?.Count ?? 0;
    }
}

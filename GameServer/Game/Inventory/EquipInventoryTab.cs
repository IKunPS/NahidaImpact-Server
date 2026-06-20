using System.Collections.Generic;
using System.Linq;
using NahidaImpact.Database.Inventory;

namespace NahidaImpact.GameServer.Game.Inventory;

/// <summary>Tab for unique items (weapons, relics) — stored as a set, each item is unique by GUID.</summary>
public class EquipInventoryTab : IInventoryTab
{
    private readonly HashSet<ItemData> _items = [];
    private readonly int _maxCapacity;

    public EquipInventoryTab(int maxCapacity)
    {
        _maxCapacity = maxCapacity;
    }

    public int Size => _items.Count;
    public int MaxCapacity => _maxCapacity;

    // Equip tabs store unique items — ID-based lookup is not meaningful
    public ItemData? GetItemById(int id) => null;

    public void OnAddItem(ItemData item) => _items.Add(item);

    public void OnRemoveItem(ItemData item) => _items.Remove(item);

    public int GetItemCountById(int itemId) =>
        (int)_items.Count(i => i.ItemId == itemId);
}

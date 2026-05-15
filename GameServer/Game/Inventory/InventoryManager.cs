using NahidaImpact.KcpSharp;
using NahidaImpact.Data;
using NahidaImpact.Database;
using NahidaImpact.Database.Inventory;
using NahidaImpact.Enums.Item;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.GameServer.Server.Packet.Send.Inventory;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Game.Inventory;

public class InventoryManager(PlayerInstance player) : BasePlayerManager(player)
{
    public InventoryData Data { get; } = DatabaseHelper.GetInstanceOrCreateNew<InventoryData>(player.Uid);

    private readonly Dictionary<ulong, ItemData> _store = [];

    public IReadOnlyDictionary<ulong, ItemData> Items => _store;

    // Virtual item limits (mirrors Java addVirtualItem)
    private static readonly int[] VirtualItemIds = [101, 102, 105, 106, 107, 121, 201, 202, 203, 204];

    public void LoadFromDatabase()
    {
        foreach (var item in Data.Items)
        {
            if (item.Guid == 0) continue;
            if (!GameData.ItemData.ContainsKey(item.ItemId)) continue;
            _store[item.Guid] = item;
        }
    }

    public ItemData? GetItemByGuid(ulong guid)
        => _store.GetValueOrDefault(guid);

    public ItemData? GetFirstItem(int itemId)
        => _store.Values.FirstOrDefault(i => i.ItemId == itemId);

    public int GetItemCountById(int itemId)
    {
        if (IsVirtualItem(itemId))
            return GetVirtualItemCount(itemId);
        var item = GetFirstItem(itemId);
        return item?.Count ?? 0;
    }

    public bool HasItem(int itemId, int minCount = 1)
        => GetItemCountById(itemId) >= minCount;

    private static bool IsVirtualItem(int itemId)
        => itemId is 101 or 102 or 105 or 106 or 107 or 121 or 201 or 202 or 203 or 204;

    public bool AddItem(int itemId, int count = 1)
    {
        if (!GameData.ItemData.TryGetValue(itemId, out var itemData))
            return false;

        var item = new ItemData
        {
            ItemId = itemId,
            Count = count
        };
        return AddItem(item);
    }

    public bool AddItem(ItemData item)
    {
        var result = PutItem(item);
        if (result != null)
        {
            _ = Player.SendPacket(new PacketStoreItemChangeNotify(result));
            return true;
        }
        return false;
    }

    public void AddItems(List<ItemData> items)
    {
        var changed = new List<ItemData>();
        foreach (var item in items)
        {
            if (item.ItemId == 0) continue;
            var result = PutItem(item);
            if (result != null)
                changed.Add(result);
        }
        if (changed.Count > 0)
            _ = Player.SendPacket(new PacketStoreItemChangeNotify(changed));
    }

    private ItemData? PutItem(ItemData item)
    {
        var data = item.GetItemData();
        if (data == null) return null;

        if (data.UseOnGain)
        {
            // UseOnGain items are consumed immediately (e.g., avatar cards, costumes)
            // TODO: Implement use item logic
            return null;
        }

        var type = data.ItemType;

        switch (type)
        {
            case ItemType.ITEM_WEAPON:
            case ItemType.ITEM_RELIQUARY:
                item.Count = 1;
                item.Guid = Player.GetNextGameGuid();
                item.OwnerId = Player.Uid;
                _store[item.Guid] = item;
                Data.Items.Add(item);
                Save();
                return item;

            case ItemType.ITEM_VIRTUAL:
                AddVirtualItem(item.ItemId, item.Count);
                return item;

            default:
                var existing = GetFirstItem(item.ItemId);
                if (existing != null)
                {
                    existing.Count = Math.Min(existing.Count + item.Count, (int)(data.StackLimit));
                    Save();
                    return existing;
                }
                else
                {
                    item.Guid = Player.GetNextGameGuid();
                    item.OwnerId = Player.Uid;
                    _store[item.Guid] = item;
                    Data.Items.Add(item);
                    Save();
                    return item;
                }
        }
    }

    private void AddVirtualItem(int itemId, int count)
    {
        switch (itemId)
        {
            case 201: // Primogem
                Player.Primogems += count;
                break;
            case 202: // Mora
                Player.Mora += count;
                break;
            case 203: // Genesis Crystal
                Player.Crystals += count;
                break;
            case 204: // Home Coin
                Player.HomeCoin += count;
                break;
        }
    }

    private int GetVirtualItemCount(int itemId)
    {
        return itemId switch
        {
            201 => Player.Primogems,
            202 => Player.Mora,
            203 => Player.Crystals,
            204 => Player.HomeCoin,
            _ => 0
        };
    }

    public bool RemoveItem(ItemData item, int count)
    {
        if (count <= 0 || item == null) return false;

        var data = item.GetItemData();
        if (data != null && data.IsEquip)
            item.Count = 0;
        else
            item.Count -= count;

        if (item.Count <= 0)
        {
            _store.Remove(item.Guid);
            Data.Items.Remove(item);
            _ = Player.SendPacket(new PacketStoreItemDelNotify(item));
        }
        else
        {
            _ = Player.SendPacket(new PacketStoreItemChangeNotify(item));
        }

        Save();
        return true;
    }

    public bool RemoveItem(ulong guid, int count = 1)
    {
        var item = GetItemByGuid(guid);
        if (item == null) return false;
        return RemoveItem(item, count);
    }

    public bool RemoveItemById(int itemId, int count)
    {
        var item = GetFirstItem(itemId);
        if (item == null) return false;
        return RemoveItem(item, count);
    }

    public bool PayItem(int itemId, int count)
    {
        if (GetItemCountById(itemId) < count) return false;

        if (IsVirtualItem(itemId))
        {
            PayVirtualItem(itemId, count);
            return true;
        }

        return RemoveItemById(itemId, count);
    }

    public bool PayItems(IEnumerable<ItemParam> costItems, int quantity = 1)
    {
        // Validate all costs first
        foreach (var cost in costItems)
        {
            if (GetItemCountById((int)cost.ItemId) < (int)cost.Count * quantity)
                return false;
        }
        // Deduct
        foreach (var cost in costItems)
            PayItem((int)cost.ItemId, (int)cost.Count * quantity);
        return true;
    }

    private void PayVirtualItem(int itemId, int count)
    {
        switch (itemId)
        {
            case 201: Player.Primogems -= count; break;
            case 202: Player.Mora -= count; break;
            case 203: Player.Crystals -= count; break;
            case 204: Player.HomeCoin -= count; break;
        }
    }

    public bool EquipItem(ulong avatarGuid, ulong equipGuid)
    {
        var avatar = Player.AvatarManager.GetAvatarByGuid(avatarGuid);
        var item = GetItemByGuid(equipGuid);
        if (avatar == null || item == null) return false;

        // Set equip character
        item.EquipCharacter = (int)avatar.AvatarId;
        Save();

        _ = Player.SendPacket(new PacketWearEquipRsp(avatarGuid, equipGuid));
        _ = Player.SendPacket(new PacketStoreItemChangeNotify(item));
        return true;
    }

    public bool UnequipItem(ulong avatarGuid, uint slot)
    {
        // Find equipped item in this slot
        var equippedItems = _store.Values
            .Where(i => i.EquipCharacter > 0)
            .ToList();

        ItemData? found = null;
        foreach (var item in equippedItems)
        {
            if (item.GetEquipSlot() == slot && item.EquipCharacter == (int)avatarGuid)
            {
                found = item;
                break;
            }
        }

        if (found == null) return false;

        found.EquipCharacter = 0;
        Save();

        _ = Player.SendPacket(new PacketTakeoffEquipRsp(avatarGuid, slot));
        _ = Player.SendPacket(new PacketStoreItemChangeNotify(found));

        // Notify equip change
        _ = Player.SendPacket(new BasePacket((ushort)CmdIds.AvatarEquipChangeNotify));
        // TODO: Send proper AvatarEquipChangeNotify

        return true;
    }

    public bool SetEquipLockState(ulong equipGuid, bool isLocked)
    {
        var item = GetItemByGuid(equipGuid);
        if (item == null) return false;

        item.Locked = isLocked;
        Save();

        _ = Player.SendPacket(new PacketSetEquipLockStateRsp(equipGuid, isLocked));
        _ = Player.SendPacket(new PacketStoreItemChangeNotify(item));
        return true;
    }

    public bool DestroyMaterial(ulong guid, int count)
    {
        return RemoveItem(guid, count);
    }
    
    public void UpgradeWeapon(ulong targetGuid, List<ulong> foodGuids, List<ItemParam> itemParams)
    {
        // TODO: Full weapon upgrade logic
        var weapon = GetItemByGuid(targetGuid);
        if (weapon == null) return;

        // Consume fodder weapons
        foreach (var guid in foodGuids)
        {
            var fodder = GetItemByGuid(guid);
            if (fodder != null)
                RemoveItem(fodder, fodder.Count);
        }

        // Consume cost items
        PayItems(itemParams);

        // Add exp (simplified)
        weapon.Level = Math.Min(weapon.Level + 1, weapon.GetItemData()?.MaxLevel ?? 90);
        Save();

        _ = Player.SendPacket(new BasePacket((ushort)CmdIds.WeaponUpgradeRsp));
    }

    public void PromoteWeapon(ulong targetGuid)
    {
        var weapon = GetItemByGuid(targetGuid);
        if (weapon == null) return;

        weapon.PromoteLevel++;
        Save();

        _ = Player.SendPacket(new BasePacket((ushort)CmdIds.WeaponPromoteRsp));
    }

    public void AwakenWeapon(ulong targetGuid, List<ulong> itemGuids)
    {
        var weapon = GetItemByGuid(targetGuid);
        if (weapon == null) return;

        // Consume fodder
        foreach (var guid in itemGuids)
            RemoveItem(guid);

        weapon.Refinement++;
        Save();

        _ = Player.SendPacket(new BasePacket((ushort)CmdIds.WeaponAwakenRsp));
    }

    public void UpgradeReliquary(ulong targetGuid, List<ulong> foodGuids, List<ItemParam> itemParams)
    {
        var relic = GetItemByGuid(targetGuid);
        if (relic == null) return;

        foreach (var guid in foodGuids)
            RemoveItem(guid);

        PayItems(itemParams);

        relic.Level = Math.Min(relic.Level + 1, relic.GetItemData()?.MaxLevel ?? 20);
        Save();

        _ = Player.SendPacket(new BasePacket((ushort)CmdIds.ReliquaryUpgradeRsp));
    }

    public void PromoteReliquary(ulong targetGuid)
    {
        var relic = GetItemByGuid(targetGuid);
        if (relic == null) return;

        relic.PromoteLevel++;
        Save();

        _ = Player.SendPacket(new BasePacket((ushort)CmdIds.ReliquaryPromoteRsp));
    }

    public void DecomposeReliquary(List<ulong> guidList)
    {
        foreach (var guid in guidList)
            RemoveItem(guid);

        _ = Player.SendPacket(new BasePacket((ushort)CmdIds.ReliquaryDecomposeRsp));
    }

    public void UseItem(ulong guid, uint count, ulong targetGuid, uint optionIdx)
    {
        var item = GetItemByGuid(guid);
        if (item == null) return;

        // TODO: Implement item use effects
        RemoveItem(item, (int)count);

        _ = Player.SendPacket(new BasePacket((ushort)CmdIds.UseItemRsp));
    }

    public void Save()
    {
        DatabaseHelper.UpdateInstance(Data);
    }
}

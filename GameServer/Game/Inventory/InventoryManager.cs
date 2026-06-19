using System;
using System.Collections.Generic;
using System.Linq;
using NahidaImpact.KcpSharp;
using NahidaImpact.Data;
using NahidaImpact.Data.Common;
using NahidaImpact.Database;
using NahidaImpact.Database.Avatar;
using NahidaImpact.Database.Inventory;
using NahidaImpact.Enums.Entity;
using NahidaImpact.Enums.Item;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.GameServer.Server.Packet.Send.Inventory;
using NahidaImpact.Proto;
using NahidaImpact.Util;

namespace NahidaImpact.GameServer.Game.Inventory;

public class InventoryManager : BasePlayerManager
{
    private const int DefaultWeaponCapacity = 200;
    private const int DefaultRelicCapacity = 1500;
    private const int DefaultMaterialCapacity = 2000;
    private const int DefaultFurnitureCapacity = 2000;

    private readonly Dictionary<ulong, ItemData> _store = [];
    private readonly Dictionary<ItemType, IInventoryTab> _tabs = [];

    public InventoryData Data { get; }
    public IReadOnlyDictionary<ulong, ItemData> Items => _store;

    public InventoryManager(PlayerInstance player) : base(player)
    {
        Data = DatabaseHelper.GetInstanceOrCreateNew<InventoryData>(player.Uid);
        EnsureTabsInitialized();
    }

    private void EnsureTabsInitialized()
    {
        if (_tabs.Count > 0) return;
        _tabs[ItemType.ITEM_WEAPON] = new EquipInventoryTab(DefaultWeaponCapacity);
        _tabs[ItemType.ITEM_RELIQUARY] = new EquipInventoryTab(DefaultRelicCapacity);
        _tabs[ItemType.ITEM_MATERIAL] = new MaterialInventoryTab(DefaultMaterialCapacity);
        _tabs[ItemType.ITEM_FURNITURE] = new MaterialInventoryTab(DefaultFurnitureCapacity);
    }

    #region Tab Access

    public IInventoryTab? GetInventoryTab(ItemType type)
    {
        EnsureTabsInitialized();
        return _tabs.GetValueOrDefault(type);
    }

    public IInventoryTab? GetTabByItemId(int itemId)
    {
        if (!GameData.ItemData.TryGetValue(itemId, out var itemData))
            return null;
        return GetInventoryTab(itemData.ItemType);
    }

    public ItemData? GetItemById(int itemId)
    {
        var tab = GetTabByItemId(itemId);
        return tab?.GetItemById(itemId);
    }

    public int GetItemCountById(int itemId)
    {
        if (IsVirtualItem(itemId))
            return GetVirtualItemCount(itemId);
        var tab = GetTabByItemId(itemId);
        return tab?.GetItemCountById(itemId) ?? 0;
    }

    #endregion

    #region Item Lookup

    public ItemData? GetItemByGuid(ulong guid) => _store.GetValueOrDefault(guid);

    public ItemData? GetFirstItem(int itemId) =>
        _store.Values.FirstOrDefault(i => i.ItemId == itemId);

    #endregion

    #region Virtual Items

    private static readonly HashSet<int> VirtualItemIds = [101, 102, 105, 106, 107, 121, 201, 202, 203, 204];

    private static bool IsVirtualItem(int itemId) => VirtualItemIds.Contains(itemId);

    private int GetVirtualItemCount(int itemId)
    {
        return itemId switch
        {
            201 => Player.Primogems,
            202 => Player.Mora,
            203 => Player.Crystals,
            106 => Player.GetProperty(Prop.PlayerProp.PROP_PLAYER_RESIN),
            107 => Player.GetProperty(Prop.PlayerProp.PROP_PLAYER_LEGENDARY_KEY),
            204 => Player.HomeCoin,
            _ => GetInventoryTab(ItemType.ITEM_MATERIAL)?.GetItemCountById(itemId) ?? 0
        };
    }

    private void AddVirtualItem(int itemId, int count)
    {
        switch (itemId)
        {
            case 101: // Character exp
                foreach (var entity in Player.TeamManager.GetActiveTeam())
                {
                    var avatar = entity?.AvatarInfo;
                    if (avatar != null)
                        UpgradeAvatarExp(avatar, count);
                }
                break;
            case 102: // Adventure exp
                Player.AddExpDirectly(count);
                break;
            case 105: // Companionship exp
                foreach (var entity in Player.TeamManager.GetActiveTeam())
                {
                    var avatar = entity?.AvatarInfo;
                    if (avatar != null)
                        UpgradeAvatarFetterExp(avatar, count);
                }
                break;
            case 106: // Resin
                Player.AddResin(count);
                break;
            case 107: // Legendary Key
                Player.AddLegendaryKey(count);
                break;
            case 121: // Home exp
                Player.AddHomeExp(count);
                break;
            case 201: Player.Primogems += count; break;
            case 202: Player.Mora += count; break;
            case 203: Player.Crystals += count; break;
            case 204: Player.HomeCoin += count; break;
        }
    }

    private ItemData? PayVirtualItem(int itemId, int count)
    {
        switch (itemId)
        {
            case 201: Player.Primogems -= count; return null;
            case 202: Player.Mora -= count; return null;
            case 203: Player.Crystals -= count; return null;
            case 106: Player.UseResin(count); return null;
            case 107: Player.UseLegendaryKey(count); return null;
            case 204: Player.HomeCoin -= count; return null;
            default:
                var gameItem = GetInventoryTab(ItemType.ITEM_MATERIAL)?.GetItemById(itemId);
                if (gameItem != null)
                    RemoveItem(gameItem, count);
                return gameItem;
        }
    }

    #endregion

    #region AddItem

    public bool AddItem(int itemId) => AddItem(itemId, 1);

    public bool AddItem(int itemId, int count) => AddItem(itemId, count, null);

    public bool AddItem(int itemId, int count, ActionReason? reason)
    {
        if (!GameData.ItemData.TryGetValue(itemId, out var itemData))
            return false;

        var item = new ItemData { ItemId = itemId, Count = count };

        if (itemData.ItemType == ItemType.ITEM_WEAPON)
            item.InitWeaponAffixes();

        return AddItem(item, reason);
    }

    public bool AddItem(ItemData item) => AddItem(item, null);

    public bool AddItem(ItemData item, ActionReason? reason)
    {
        EnsureTabsInitialized();
        var result = PutItem(item);

        if (result != null)
        {
            TriggerAddItemEvents(result);
            _ = Player.SendPacket(new PacketStoreItemChangeNotify(result));
            if (reason != null)
                _ = Player.SendPacket(new PacketItemAddHintNotify(result, (uint)reason.Value));
            return true;
        }

        return false;
    }

    public bool AddItem(ItemParamData itemParam) => AddItem(itemParam, null);

    public bool AddItem(ItemParamData itemParam, ActionReason? reason)
    {
        if (itemParam == null) return false;
        return AddItem(itemParam.Id, itemParam.Count, reason);
    }

    public int AddItems(List<ItemData> items) => AddItems(items, null);

    /// <returns>Number of items actually added to inventory.</returns>
    public int AddItems(List<ItemData> items, ActionReason? reason)
    {
        EnsureTabsInitialized();
        var changed = new List<ItemData>();
        foreach (var item in items)
        {
            if (item.ItemId == 0) continue;
            var result = PutItem(item);
            if (result != null)
            {
                TriggerAddItemEvents(result);
                changed.Add(result);
            }
        }
        if (changed.Count > 0)
        {
            _ = Player.SendPacket(new PacketStoreItemChangeNotify(changed));

            // Send hint notify in batches to avoid oversized packets
            if (reason != null)
            {
                const int hintBatchSize = 100;
                for (int i = 0; i < changed.Count; i += hintBatchSize)
                {
                    var batch = changed.Skip(i).Take(hintBatchSize).ToList();
                    _ = Player.SendPacket(new PacketItemAddHintNotify(batch, (uint)reason.Value));
                }
            }
        }
        return changed.Count;
    }

    public void AddItemParams(IEnumerable<ItemParam> itemParams)
    {
        AddItems(itemParams.Select(p => new ItemData { ItemId = (int)p.ItemId, Count = (int)p.Count }).ToList(), null);
    }

    public void AddItemParamDatas(IEnumerable<ItemParamData> itemParams, ActionReason? reason = null)
    {
        AddItems(itemParams.Select(p => new ItemData { ItemId = p.Id, Count = p.Count }).ToList(), reason);
    }

    #endregion

    #region PutItem

    /// <summary>Core placement logic - dispatches by item type, manages stacking and tab capacity.</summary>
    private ItemData? PutItem(ItemData item)
    {
        var data = item.GetItemData();
        if (data == null) return null;

        // Track item obtain history
        try { Player.ProgressManager?.AddItemObtainedHistory(item.ItemId, item.Count); }
        catch { /* non-critical */ }

        if (data.UseOnGain)
        {
            UseItemDirect(data);
            return null;
        }

        var type = data.ItemType;
        var tab = GetInventoryTab(type);

        switch (type)
        {
            case ItemType.ITEM_WEAPON:
            case ItemType.ITEM_RELIQUARY:
                if (tab != null && tab.Size >= tab.MaxCapacity)
                    return null;
                item.Count = 1;
                item.Level = item.Level > 0 ? item.Level : 1;
                if (type == ItemType.ITEM_WEAPON && item.Affixes.Count == 0)
                    item.InitWeaponAffixes();
                PutNewItem(item, tab);
                return item;

            case ItemType.ITEM_VIRTUAL:
                AddVirtualItem(item.ItemId, item.Count);
                return item;

            default:
                switch (data.MaterialType)
                {
                    case MaterialType.MATERIAL_AVATAR:
                    case MaterialType.MATERIAL_FLYCLOAK:
                    case MaterialType.MATERIAL_COSTUME:
                    case MaterialType.MATERIAL_NAMECARD:
                        Logger.GetByClassName().Warn(
                            $"Material type {data.MaterialType} for item {item.ItemId} lacking useOnGain");
                        return null;
                }

                if (tab == null) return null;

                var existing = tab.GetItemById(item.ItemId);
                if (existing == null)
                {
                    if (tab.Size >= tab.MaxCapacity)
                        return null;
                    PutNewItem(item, tab);
                    return item;
                }
                else
                {
                    if (existing.Count >= data.StackLimit)
                        return null;
                    existing.Count = Math.Min(existing.Count + item.Count, (int)data.StackLimit);
                    Save();
                    return existing;
                }
        }
    }

    private void PutNewItem(ItemData item, IInventoryTab? tab)
    {
        item.OwnerId = Player.Uid;
        item.Guid = Player.GetNextGameGuid();

        // Mark as new if no other stack of this item exists
        if (tab != null && tab.GetItemById(item.ItemId) == null)
            item.IsNew = true;

        _store[item.Guid] = item;
        tab?.OnAddItem(item);
        Data.Items.Add(item);
        Save();
    }

    /// <summary>Direct-load item from DB at login - bypasses runtime checks.</summary>
    public void LoadItem(ItemData item)
    {
        EnsureTabsInitialized();
        if (item.GetItemData() == null) return;

        var tab = GetInventoryTab(item.ItemType);
        item.OwnerId = Player.Uid;
        _store[item.Guid] = item;
        tab?.OnAddItem(item);
    }

    #endregion

    #region Events

    private static void TriggerAddItemEvents(ItemData result)
    {
        // Stub: trigger BattlePass missions and Quest events
    }

    private static void TriggerRemItemEvents(ItemData item, int removeCount)
    {
        // Stub: trigger BattlePass missions and Quest events
    }

    #endregion

    #region UseItemDirect

    /// <summary>Handle UseOnGain items immediately.</summary>
    public void UseItemDirect(Data.Excel.ItemDataExcel itemData)
    {
        if (itemData.ItemUseActions == null || itemData.ItemUseActions.Count == 0)
            return;

        foreach (var action in itemData.ItemUseActions)
        {
            switch (action.UseOp)
            {
                case "ITEM_USE_GAIN_AVATAR":
                    if (action.UseParam.Count > 0 && int.TryParse(action.UseParam[0], out var avatarId))
                        _ = Player.AvatarManager.CreateAvatar(avatarId);
                    break;
                case "ITEM_USE_GAIN_FLYCLOAK":
                    if (action.UseParam.Count > 0 && int.TryParse(action.UseParam[0], out var flycloakId))
                    {
                        if (!Player.FlyCloakList.Contains(flycloakId))
                            Player.FlyCloakList.Add(flycloakId);
                    }
                    break;
                case "ITEM_USE_GAIN_COSTUME":
                    if (action.UseParam.Count > 0 && int.TryParse(action.UseParam[0], out var costumeId))
                    {
                        var costumes = Player.GetCostumeList();
                        if (!costumes.Contains(costumeId))
                            costumes.Add(costumeId);
                    }
                    break;
                case "ITEM_USE_GAIN_NAME_CARD":
                    if (action.UseParam.Count > 0 && int.TryParse(action.UseParam[0], out var nameCardId))
                    {
                        if (!Player.NameCardList.Contains(nameCardId))
                            Player.NameCardList.Add(nameCardId);
                    }
                    break;
                case "ITEM_USE_ADD_ITEM":
                    if (action.UseParam.Count >= 2
                        && int.TryParse(action.UseParam[0], out var grantItemId)
                        && int.TryParse(action.UseParam[1], out var grantCount))
                        AddItem(grantItemId, grantCount);
                    break;
            }
        }
    }

    #endregion

    #region HasItem / Check

    public bool HasItem(int itemId, int minCount = 1) =>
        GetItemCountById(itemId) >= minCount;

    public bool HasItem(int itemId, int count, bool enforce)
    {
        var item = GetFirstItem(itemId);
        if (item == null) return false;
        return enforce ? item.Count == count : item.Count >= count;
    }

    public bool HasAllItems(IEnumerable<ItemParam> items)
    {
        foreach (var item in items)
        {
            if (!HasItem((int)item.ItemId, (int)item.Count))
                return false;
        }
        return true;
    }

    #endregion

    #region RemoveItem

    public bool RemoveItem(ItemData item, int count)
    {
        if (count <= 0 || item == null) return false;

        var data = item.GetItemData();
        if (data != null && data.IsEquip)
            item.Count = 0;
        else
            item.Count -= count;

        var removeCount = Math.Min(count, item.Count >= 0 ? count : count + item.Count);

        if (item.Count <= 0)
        {
            DeleteItem(item);
            _ = Player.SendPacket(new PacketStoreItemDelNotify(item));
        }
        else
        {
            _ = Player.SendPacket(new PacketStoreItemChangeNotify(item));
        }

        TriggerRemItemEvents(item, removeCount);
        Save();
        return true;
    }

    public bool RemoveItem(ulong guid, int count = 1)
    {
        var item = GetItemByGuid(guid);
        return item != null && RemoveItem(item, count);
    }

    public bool RemoveItemById(int itemId, int count)
    {
        var item = GetFirstItem(itemId);
        return item != null && RemoveItem(item, count);
    }

    public bool RemoveItem(int itemId, int count)
    {
        var item = GetFirstItem(itemId);
        return item != null && RemoveItem(item, count);
    }

    public void RemoveItems(List<ItemData> items)
    {
        foreach (var item in items)
            RemoveItem(item, item.Count);
    }

    public void RemoveItems(IEnumerable<ItemParam> items)
    {
        foreach (var entry in items)
            RemoveItem((int)entry.ItemId, (int)entry.Count);
    }

    private void DeleteItem(ItemData item)
    {
        _store.Remove(item.Guid);
        Data.Items.Remove(item);

        IInventoryTab? tab = null;
        if (item.GetItemData() != null)
            tab = GetInventoryTab(item.ItemType);
        tab?.OnRemoveItem(item);
    }

    #endregion

    #region PayItem

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

    public bool PayItem(ItemParamData costItem) => PayItem(costItem.Id, costItem.Count);

    public bool PayItems(IEnumerable<ItemParam> costItems, int quantity = 1)
    {
        foreach (var cost in costItems)
        {
            if (GetItemCountById((int)cost.ItemId) < (int)cost.Count * quantity)
                return false;
        }
        foreach (var cost in costItems)
            PayItem((int)cost.ItemId, (int)cost.Count * quantity);
        return true;
    }

    public bool PayItems(IEnumerable<ItemParamData> costItems, int quantity = 1)
    {
        foreach (var cost in costItems)
        {
            if (GetItemCountById(cost.Id) < cost.Count * quantity)
                return false;
        }
        foreach (var cost in costItems)
            PayItem(cost.Id, cost.Count * quantity);
        return true;
    }

    #endregion

    #region Equip / Unequip

    public bool EquipItem(ulong avatarGuid, ulong equipGuid)
    {
        var avatar = Player.AvatarManager.GetAvatarByGuid(avatarGuid);
        var item = GetItemByGuid(equipGuid);
        if (avatar == null || item == null) return false;

        var itemType = item.ItemType;
        var equipSlot = item.GetEquipSlot();

        if (equipSlot > 0)
            UnequipSlotFromAvatar(avatar, equipSlot);

        if (item.EquipCharacter > 0 && item.EquipCharacter != (int)avatar.AvatarId)
        {
            var prevAvatar = Player.AvatarManager.GetAvatarById((uint)item.EquipCharacter);
            if (prevAvatar != null)
                UnequipSlotFromAvatar(prevAvatar, equipSlot);
        }

        item.EquipCharacter = (int)avatar.AvatarId;
        avatar.WeaponGuid = item.Guid;
        avatar.WeaponId = (uint)item.ItemId;

        if (itemType == ItemType.ITEM_WEAPON)
        {
            var scene = Player.Scene;
            if (scene != null)
            {
                var gadgetId = (int)(item.GetItemData()?.GadgetId ?? 0);
                if (gadgetId > 0)
                {
                    var weaponEntity = new EntityWeapon(scene, gadgetId)
                    {
                        ItemId = item.ItemId,
                        ItemGuid = item.Guid
                    };
                    item.WeaponEntityId = (int)weaponEntity.Id;
                    scene.WeaponEntities[(int)weaponEntity.Id] = weaponEntity;
                }
            }
        }

        Save();

        _ = Player.SendPacket(new PacketWearEquipRsp(avatarGuid, equipGuid));
        _ = Player.SendPacket(new PacketStoreItemChangeNotify(item));
        _ = Player.SendPacket(new PacketAvatarEquipChangeNotify(avatarGuid, item));

        avatar.RecalcStats(item);
        return true;
    }

    public bool UnequipItem(ulong avatarGuid, uint slot)
    {
        var avatar = Player.AvatarManager.GetAvatarByGuid(avatarGuid);
        if (avatar == null) return false;

        if (slot == 6)
        {
            var item = _store.Values.FirstOrDefault(i =>
                i.EquipCharacter == (int)avatar.AvatarId && i.ItemType == ItemType.ITEM_WEAPON);
            if (item == null) return false;
            return UnequipSlotFromAvatar(avatar, 6);
        }

        var equippedItem = _store.Values.FirstOrDefault(i =>
            i.EquipCharacter == (int)avatar.AvatarId && i.GetEquipSlot() == (int)slot);

        if (equippedItem == null) return false;
        return UnequipSlotFromAvatar(avatar, (int)slot);
    }

    private bool UnequipSlotFromAvatar(AvatarDataInfo avatar, int equipSlot)
    {
        ItemData? item;
        if (equipSlot == 6)
            item = _store.Values.FirstOrDefault(i =>
                i.EquipCharacter == (int)avatar.AvatarId && i.ItemType == ItemType.ITEM_WEAPON);
        else
            item = _store.Values.FirstOrDefault(i =>
                i.EquipCharacter == (int)avatar.AvatarId && i.GetEquipSlot() == equipSlot);

        if (item == null) return false;

        if (item.ItemType == ItemType.ITEM_WEAPON && item.WeaponEntityId > 0)
        {
            var scene = Player.Scene;
            scene?.WeaponEntities.TryRemove(item.WeaponEntityId, out _);
        }

        item.EquipCharacter = 0;
        item.WeaponEntityId = 0;
        Save();

        _ = Player.SendPacket(new PacketTakeoffEquipRsp(avatar.Guid, (uint)equipSlot));
        _ = Player.SendPacket(new PacketStoreItemChangeNotify(item));
        _ = Player.SendPacket(new PacketAvatarEquipChangeNotify(avatar.Guid, (uint)equipSlot));

        avatar.RecalcStats(null);
        return true;
    }

    public bool SetEquipLockState(ulong equipGuid, bool isLocked)
    {
        var item = GetItemByGuid(equipGuid);
        if (item == null) return false;

        if (!isLocked)
            item.Favourite = false;

        item.Locked = isLocked;
        Save();

        _ = Player.SendPacket(new PacketSetEquipLockStateRsp(equipGuid, isLocked));
        _ = Player.SendPacket(new PacketStoreItemChangeNotify(item));
        return true;
    }

    #endregion

    #region Database

    public void LoadFromDatabase()
    {
        EnsureTabsInitialized();

        foreach (var item in Data.Items)
        {
            if (item.Guid == 0) continue;

            var itemData = item.GetItemData();
            if (itemData == null) continue;

            LoadItem(item);

            if (item.IsEquipped)
            {
                var avatar = Player.AvatarManager.GetAvatarById((uint)item.EquipCharacter);
                if (avatar != null)
                {
                    RelinkEquipToAvatar(avatar, item);
                }
                else
                {
                    item.EquipCharacter = 0;
                    Save();
                }
            }
        }
    }

    private void RelinkEquipToAvatar(AvatarDataInfo avatar, ItemData item)
    {
        if (item.ItemType != ItemType.ITEM_WEAPON) return;

        var scene = Player.Scene;
        if (scene == null) return;

        var gadgetId = (int)(item.GetItemData()?.GadgetId ?? 0);
        if (gadgetId <= 0) return;

        item.WeaponEntityId = (int)scene.World.GetNextEntityId(EntityIdTypeEnum.Weapon);
    }

    #endregion

    #region Destroy / Use / Delegate

    public bool DestroyMaterial(ulong guid, int count)
    {
        var item = GetItemByGuid(guid);
        if (item == null || !item.IsDestroyable) return false;

        var removeAmount = Math.Min(count, item.Count);
        RemoveItem(item, removeAmount);

        // Return destroy materials
        var data = item.GetItemData();
        if (data?.DestroyReturnMaterial is { Count: > 0 })
        {
            for (int i = 0; i < data.DestroyReturnMaterial.Count; i++)
            {
                var returnId = data.DestroyReturnMaterial[i];
                var returnCount = i < data.DestroyReturnMaterialCount.Count
                    ? data.DestroyReturnMaterialCount[i] : 1;
                AddItem(returnId, returnCount * removeAmount);
            }
        }

        return true;
    }

    public void UseItem(ulong guid, uint count, ulong targetGuid, uint optionIdx)
    {
        var item = GetItemByGuid(guid);
        if (item == null) return;

        var itemData = item.GetItemData();
        if (itemData == null) return;
        if (item.Count < count) return;

        if (UseItemWithActions(itemData, targetGuid, count, optionIdx))
        {
            RemoveItem(item, (int)count);
            _ = Player.SendPacket(new BasePacket((ushort)CmdIds.UseItemRsp));
        }
    }

    private bool UseItemWithActions(Data.Excel.ItemDataExcel itemData, ulong targetGuid, uint count, uint optionIdx)
    {
        if (itemData.ItemUseActions == null || itemData.ItemUseActions.Count == 0)
            return true;

        bool anySucceeded = false;
        foreach (var action in itemData.ItemUseActions)
        {
            switch (action.UseOp)
            {
                case "ITEM_USE_GAIN_AVATAR":
                    if (action.UseParam.Count > 0 && int.TryParse(action.UseParam[0], out var avatarId))
                    {
                        _ = Player.AvatarManager.CreateAvatar(avatarId);
                        anySucceeded = true;
                    }
                    break;
                case "ITEM_USE_ADD_ITEM":
                    if (action.UseParam.Count >= 2
                        && int.TryParse(action.UseParam[0], out var grantItemId)
                        && int.TryParse(action.UseParam[1], out var grantCount))
                    {
                        AddItem(grantItemId, grantCount * (int)count);
                        anySucceeded = true;
                    }
                    break;
                case "ITEM_USE_ADD_EXP":
                case "ITEM_USE_ADD_WEAPON_EXP":
                case "ITEM_USE_ADD_RELIQUARY_EXP":
                    anySucceeded = true;
                    break;
                default:
                    anySucceeded = true;
                    break;
            }
        }

        return anySucceeded;
    }

    public void FavouriteEquip(ulong itemGuid, bool isFavourite)
    {
        var equip = GetItemByGuid(itemGuid);
        if (equip == null) return;

        if (isFavourite)
            equip.Locked = true;

        equip.Favourite = isFavourite;
        Save();

        _ = Player.SendPacket(new PacketStoreItemChangeNotify(equip));
    }

    #endregion

    #region Relic

    public void UpgradeReliquary(ulong targetGuid, List<ulong> foodGuids, List<ItemParam> itemParams)
    {
        var relic = GetItemByGuid(targetGuid);
        if (relic == null || relic.ItemType != ItemType.ITEM_RELIQUARY) return;

        int moraCost = 0;
        int expGain = 0;
        var foodRelics = new List<ItemData>();

        foreach (var guid in foodGuids)
        {
            var food = GetItemByGuid(guid);
            if (food == null || !food.IsDestroyable) continue;

            int foodExp = food.GetItemData()?.BaseConvExp ?? 0;
            moraCost += foodExp;
            expGain += foodExp;
            if (food.TotalExp > 0)
                expGain += (food.TotalExp * 4) / 5;
            foodRelics.Add(food);
        }

        foreach (var param in itemParams)
        {
            var data = GameData.ItemData.GetValueOrDefault((int)param.ItemId);
            if (data?.ItemUseActions != null)
            {
                int gain = 0;
                foreach (var action in data.ItemUseActions)
                {
                    if (action.UseOp == "ITEM_USE_ADD_RELIQUARY_EXP" && action.UseParam.Count > 0)
                    {
                        if (int.TryParse(action.UseParam[0], out var expVal))
                            gain += expVal;
                    }
                }
                gain *= (int)param.Count;
                expGain += gain;
                moraCost += gain;
            }
        }

        if (expGain <= 0) return;

        var payList = new List<ItemParamData> { new(202, moraCost) };
        foreach (var param in itemParams)
            payList.Add(new ItemParamData((int)param.ItemId, (int)param.Count));

        if (!PayItems(payList)) return;

        RemoveItems(foodRelics);

        // Random rate boost: 1% for x5, 9% for x2
        int rate = 1;
        int boost = Random.Shared.Next(1, 101);
        if (boost == 100) rate = 5;
        else if (boost <= 9) rate = 2;
        expGain *= rate;

        var relicData = relic.GetItemData();
        int level = relic.Level;
        int oldLevel = level;
        int exp = relic.Exp;
        int totalExp = relic.TotalExp;
        int maxLevel = relicData?.MaxLevel ?? 20;
        int reqExp = GetRelicExpRequired((int)(relicData?.RankLevel ?? 1), level);
        int upgrades = 0;

        while (expGain > 0 && reqExp > 0 && level < maxLevel)
        {
            int toGain = Math.Min(expGain, reqExp - exp);
            exp += toGain;
            totalExp += toGain;
            expGain -= toGain;

            if (exp >= reqExp)
            {
                exp = 0;
                level++;
                if (relicData?.CanAddRelicProp(level) == true)
                    upgrades++;
                reqExp = GetRelicExpRequired((int)(relicData?.RankLevel ?? 1), level);
            }
        }

        relic.Level = level;
        relic.Exp = exp;
        relic.TotalExp = totalExp;
        Save();

        if (oldLevel != level && relic.EquipCharacter > 0)
        {
            var avatar = Player.AvatarManager.GetAvatarById((uint)relic.EquipCharacter);
            avatar?.RecalcStats(null);
        }

        _ = Player.SendPacket(new PacketStoreItemChangeNotify(relic));
        _ = Player.SendPacket(new BasePacket((ushort)CmdIds.ReliquaryUpgradeRsp));
    }

    /// <summary>Calculate required exp for relic level-up. Falls back to simple formula.</summary>
    private static int GetRelicExpRequired(int rankLevel, int level)
    {
        // Approximate formula matching official data
        return level * 100 + rankLevel * 50;
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

    #endregion

    #region Weapon Delegates

    public void UpgradeWeapon(ulong targetGuid, List<ulong> foodGuids, List<ItemParam> itemParams)
    {
        Player.WeaponManager.UpgradeWeapon(targetGuid, foodGuids, itemParams);
    }

    public void PromoteWeapon(ulong targetGuid)
    {
        Player.WeaponManager.PromoteWeapon(targetGuid);
    }

    public void AwakenWeapon(ulong targetGuid, List<ulong> itemGuids)
    {
        Player.WeaponManager.AwakenWeapon(targetGuid, itemGuids);
    }

    #endregion

    #region Persistence

    public void Save()
    {
        DatabaseHelper.UpdateInstance(Data);
    }

    #endregion

    #region Avatar Exp Helpers (virtual items 101, 105)

    private void UpgradeAvatarExp(AvatarDataInfo avatar, int expGain)
    {
        // TODO: implement when AvatarSystem is available
    }

    private void UpgradeAvatarFetterExp(AvatarDataInfo avatar, int expGain)
    {
        // TODO: implement when companionship system is available
    }

    #endregion
}

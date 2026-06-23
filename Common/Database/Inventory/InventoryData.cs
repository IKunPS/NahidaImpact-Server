using System.Collections.Generic;
using System.Linq;
using SqlSugar;
using NahidaImpact.Data;
using NahidaImpact.Data.Common;
using NahidaImpact.Data.Excel;
using NahidaImpact.Enums.Item;

namespace NahidaImpact.Database.Inventory;

[SugarTable("InventoryData")]
public class InventoryData : BaseDatabaseDataHelper
{
    [SugarColumn(IsJson = true)] public List<ItemData> Items { get; set; } = [];

    public static InventoryData? GetInventoryDataByUid(int uid, bool forceReload = false)
    {
        return DatabaseHelper.GetInstance<InventoryData>(uid, forceReload);
    }

    public static InventoryData GetOrCreateInventoryData(int uid)
    {
        return DatabaseHelper.GetInstanceOrCreateNew<InventoryData>(uid);
    }

    public static void SaveInventoryData(InventoryData data)
    {
        DatabaseHelper.UpdateInstance(data);
    }
}

public class ItemData
{
    public ulong Guid { get; set; }
    public int OwnerId { get; set; }
    public int ItemId { get; set; }
    public int Count { get; set; }
    public int Level { get; set; } = 1;
    public int Exp { get; set; }
    public int TotalExp { get; set; }
    public int PromoteLevel { get; set; }
    public bool Favourite { get; set; }
    public bool Locked { get; set; }
    public int WeaponSkinId { get; set; }
    public List<int> Affixes { get; set; } = [];
    public int Refinement { get; set; }
    public int MainPropId { get; set; }
    public List<int> AppendPropIdList { get; set; } = [];
    public int EquipCharacter { get; set; }

    // Transient fields — not persisted to DB
    [SugarColumn(IsIgnore = true)] public int WeaponEntityId { get; set; }
    [SugarColumn(IsIgnore = true)] public bool IsNew { get; set; }

    public bool IsEquipped => EquipCharacter > 0;
    public bool IsLocked => Locked;
    public bool IsDestroyable => !Locked && !IsEquipped;

    public ItemType ItemType
    {
        get
        {
            var data = ItemDataExcel;
            return data?.ItemType ?? ItemType.ITEM_NONE;
        }
    }

    public ItemDataExcel? ItemDataExcel
    {
        get
        {
            GameData.ItemData.TryGetValue(ItemId, out var data);
            return data;
        }
    }

    public bool IsStackable => ItemType is not ItemType.ITEM_WEAPON and not ItemType.ITEM_RELIQUARY;
    public bool IsEnhanceable => ItemType is ItemType.ITEM_WEAPON or ItemType.ITEM_RELIQUARY;
    public bool IsExpItem => ItemDataExcel?.MaterialType is MaterialType.MATERIAL_WEAPON_EXP_STONE
        or MaterialType.MATERIAL_AVATAR_MATERIAL
        or MaterialType.MATERIAL_RELIQUARY_MATERIAL;

    public static int GetMinPromoteLevel(int level)
    {
        if (level > 80) return 6;
        if (level > 70) return 5;
        if (level > 60) return 4;
        if (level > 50) return 3;
        if (level > 40) return 2;
        if (level > 20) return 1;
        return 0;
    }

    public EquipType EquipType
    {
        get
        {
            var data = ItemDataExcel;
            return data?.EquipTypeStr switch
            {
                "EQUIP_BRACER" => EquipType.EquipBracer,
                "EQUIP_NECKLACE" => EquipType.EquipNecklace,
                "EQUIP_SHOES" => EquipType.EquipShoes,
                "EQUIP_RING" => EquipType.EquipRing,
                "EQUIP_DRESS" => EquipType.EquipDress,
                "EQUIP_WEAPON" => EquipType.EquipWeapon,
                _ => EquipType.EquipNone
            };
        }
    }

    public int EquipSlot => (int)EquipType;

    /// <summary>Generate initial random affixes from item data's skillAffix list.</summary>
    public void InitWeaponAffixes()
    {
        var data = ItemDataExcel;
        if (data?.SkillAffix == null) return;
        Affixes = data.SkillAffix.Where(a => a > 0).ToList();
    }

    public ItemParamData ToItemParamData() => new(ItemId, Count);
}

public static class InventoryDataExtensions
{
    public static ItemData? FindByEquipCharacter(this InventoryData data, int avatarId, EquipType slot)
        => data.Items.FirstOrDefault(i => i.EquipCharacter == avatarId && i.EquipType == slot);

    public static ItemData? FindWeapon(this InventoryData data, int avatarId)
        => data.Items.FirstOrDefault(i => i.EquipCharacter == avatarId && i.ItemType == ItemType.ITEM_WEAPON);

    public static List<ItemData> FindRelics(this InventoryData data, int avatarId)
        => data.Items.Where(i => i.EquipCharacter == avatarId && i.ItemType == ItemType.ITEM_RELIQUARY).ToList();

    public static int GetItemCount(this InventoryData data, int itemId)
        => data.Items.Where(i => i.ItemId == itemId).Sum(i => i.Count);
}

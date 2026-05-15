using SqlSugar;
using NahidaImpact.Data;
using NahidaImpact.Data.Excel;
using NahidaImpact.Enums.Item;

namespace NahidaImpact.Database.Inventory;

[SugarTable("InventoryData")]
public class InventoryData : BaseDatabaseDataHelper
{
    [SugarColumn(IsJson = true)] public List<ItemData> Items { get; set; } = [];
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

    public bool IsEquipped => EquipCharacter > 0;
    public bool IsLocked => Locked;
    public bool IsDestroyable => !Locked && !IsEquipped;

    public ItemType ItemType
    {
        get
        {
            var data = GetItemData();
            return data?.ItemType ?? ItemType.ITEM_NONE;
        }
    }

    public ItemDataExcel? GetItemData()
    {
        GameData.ItemData.TryGetValue(ItemId, out var data);
        return data;
    }

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

    public int GetEquipSlot()
    {
        var data = GetItemData();
        return data?.EquipTypeStr switch
        {
            "EQUIP_BRACER" => 1,
            "EQUIP_NECKLACE" => 2,
            "EQUIP_SHOES" => 3,
            "EQUIP_RING" => 4,
            "EQUIP_DRESS" => 5,
            _ => 0
        };
    }
}

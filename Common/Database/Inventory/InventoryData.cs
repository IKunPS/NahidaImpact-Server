using SqlSugar;

namespace NahidaImpact.Database.Inventory;

[SugarTable("InventoryData")]
public class InventoryData : BaseDatabaseDataHelper
{
    [SugarColumn(IsJson = true)] public List<ItemData> MaterialItems { get; set; } = [];

    [SugarColumn(IsJson = true)] public List<ItemData> WeaponItems { get; set; } = [];
}

public class ItemData
{
    public int UniqueId { get; set; }
    public int ItemId { get; set; }
    public int Count { get; set; }
    public int Level { get; set; }
    public int Exp { get; set; }
    public int EquipAvatar { get; set; }
}
namespace NahidaImpact.Enums.Item;

public static class ItemTypeStr
{
    public static ItemType Parse(string str) => str switch
    {
        "ITEM_VIRTUAL" => ItemType.ITEM_VIRTUAL,
        "ITEM_MATERIAL" => ItemType.ITEM_MATERIAL,
        "ITEM_RELIQUARY" => ItemType.ITEM_RELIQUARY,
        "ITEM_WEAPON" => ItemType.ITEM_WEAPON,
        "ITEM_DISPLAY" => ItemType.ITEM_DISPLAY,
        "ITEM_FURNITURE" => ItemType.ITEM_FURNITURE,
        _ => ItemType.ITEM_NONE
    };
}

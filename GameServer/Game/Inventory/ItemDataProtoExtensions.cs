using NahidaImpact.Database.Inventory;
using NahidaImpact.Enums.Item;

namespace NahidaImpact.GameServer.Game.Inventory;

public static class ItemDataProtoExtensions
{
    public static Item ToProto(this ItemData item)
    {
        var proto = new Item
        {
            Guid = item.Guid,
            ItemId = (uint)item.ItemId
        };

        switch (item.ItemType)
        {
            case ItemType.ITEM_WEAPON:
                var weapon = new Weapon
                {
                    Level = (uint)item.Level,
                    Exp = (uint)item.Exp,
                    PromoteLevel = (uint)item.PromoteLevel
                };
                if (item.Affixes.Count > 0)
                {
                    foreach (var affix in item.Affixes)
                        weapon.AffixMap[(uint)affix] = (uint)item.Refinement;
                }
                proto.Equip = new Equip
                {
                    Weapon = weapon,
                    IsLocked = item.Locked
                };
                break;

            case ItemType.ITEM_RELIQUARY:
                var relic = new Reliquary
                {
                    Level = (uint)item.Level,
                    Exp = (uint)item.Exp,
                    PromoteLevel = (uint)item.PromoteLevel,
                    MainPropId = (uint)item.MainPropId
                };
                relic.AppendPropIdList.AddRange(item.AppendPropIdList.Select(x => (uint)x));
                proto.Equip = new Equip
                {
                    Reliquary = relic,
                    IsLocked = item.Locked
                };
                break;

            case ItemType.ITEM_FURNITURE:
                proto.Furniture = new Furniture { Count = (uint)item.Count };
                break;

            default:
                proto.Material = new Material { Count = (uint)item.Count };
                break;
        }

        return proto;
    }

    public static ItemHint ToItemHintProto(this ItemData item)
    {
        return new ItemHint
        {
            ItemId = (uint)item.ItemId,
            Guid = item.Guid,
            Count = (uint)item.Count,
            IsNew = item.IsNew
        };
    }

    public static SceneWeaponInfo CreateSceneWeaponInfo(this ItemData item, uint entityId)
    {
        var info = new SceneWeaponInfo
        {
            EntityId = entityId,
            ItemId = (uint)item.ItemId,
            Guid = item.Guid,
            Level = (uint)item.Level,
            GadgetId = item.ItemDataExcel?.GadgetId ?? 0,
            PromoteLevel = (uint)item.PromoteLevel,
            WeaponSkinId = (uint)item.WeaponSkinId
        };

        if (item.Affixes.Count > 0)
        {
            foreach (var affix in item.Affixes)
                info.AffixMap[(uint)affix] = (uint)item.Refinement;
        }

        return info;
    }

    public static SceneReliquaryInfo CreateSceneReliquaryInfo(this ItemData item)
    {
        return new SceneReliquaryInfo
        {
            ItemId = (uint)item.ItemId,
            Guid = item.Guid,
            Level = (uint)item.Level,
            PromoteLevel = (uint)item.PromoteLevel
        };
    }

    public static ItemParam ToItemParam(this ItemData item)
    {
        return new ItemParam
        {
            ItemId = (uint)item.ItemId,
            Count = (uint)item.Count
        };
    }
}

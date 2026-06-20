using NahidaImpact.Database.Inventory;
using NahidaImpact.GameServer.Game.Inventory;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Inventory;

public class PacketAvatarEquipChangeNotify : BasePacket
{
    /// <summary>Equip notify with full weapon/relic info.</summary>
    public PacketAvatarEquipChangeNotify(ulong avatarGuid, ItemData item) : base(CmdIds.AvatarEquipChangeNotify)
    {
        var notify = new AvatarEquipChangeNotify
        {
            AvatarGuid = avatarGuid,
            EquipGuid = item.Guid,
            ItemId = (uint)item.ItemId,
            EquipType = (uint)item.GetEquipSlot()
        };

        if (item.ItemType == Enums.Item.ItemType.ITEM_WEAPON)
            notify.Weapon = item.CreateSceneWeaponInfo((uint)item.WeaponEntityId);
        else
            notify.Reliquary = item.CreateSceneReliquaryInfo();

        SetData(notify);
    }

    /// <summary>Unequip notify with just the slot info.</summary>
    public PacketAvatarEquipChangeNotify(ulong avatarGuid, uint equipType) : base(CmdIds.AvatarEquipChangeNotify)
    {
        SetData(new AvatarEquipChangeNotify
        {
            AvatarGuid = avatarGuid,
            EquipType = equipType
        });
    }
}

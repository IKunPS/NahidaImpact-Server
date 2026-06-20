using System.Collections.Generic;
using System.Threading.Tasks;
using NahidaImpact.Data;
using NahidaImpact.Database.Avatar;
using NahidaImpact.Database.Inventory;
using NahidaImpact.Enums.Item;
using NahidaImpact.Enums.Player;
using NahidaImpact.GameServer.Server.Packet.Send.Avatar;
using NahidaImpact.GameServer.Server.Packet.Send.Inventory;
using NahidaImpact.GameServer.Server.Packet.Send.Team;
using NahidaImpact.Internationalization;
using NahidaImpact.Util;

namespace NahidaImpact.GameServer.Command.Commands;

[CommandInfo("giveall", "Game.Command.GiveAll.Desc", "Game.Command.GiveAll.Usage", ["ga"], [PermEnum.Admin])]
public class CommandGiveAll : ICommand
{
    private const int ItemsPerBatch = 200;

    private static readonly int[] Essentials =
    [
        201, // Primogem
        202, // Mora
        203, // Genesis Crystal
        204, // Home Coin
        106, // Resin
        221, // Intertwined Fate
        222, // Acquaint Fate
        223, // Starglitter
        224, // Stardust
    ];

    // Weapon ID range: 11101-16504 (all extant weapons)
    private static bool IsValidWeaponId(int id) => id >= 11101 && id <= 16504;

    // Reliquary ID range
    private static bool IsValidRelicId(int id) => id >= 20002 && id <= 99999;

    // Meaningful material items start above this
    private const int MinMaterialId = 100_000;

    [CommandDefault]
    public static async ValueTask Execute(CommandArg arg)
    {
        if (!await arg.CheckTarget()) return;
        if (!await arg.CheckOnlineTarget()) return;

        var player = arg.Target!.Player!;

        if (arg.Args.Count < 1)
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.GiveAll.Usage"));
            return;
        }

        var type = arg.Args[0].ToLower();
        int amount = arg.Args.Count >= 2 ? arg.GetInt(1) : 1;
        if (amount <= 0) amount = 1;

        switch (type)
        {
            case "avatars":
            case "ava":
            case "av":
                await GiveAllAvatars(player, arg);
                break;

            case "mats":
            case "materials":
            case "mat":
                await GiveAllByType(player, ItemType.ITEM_MATERIAL, amount, arg);
                break;

            case "weapons":
            case "wep":
            case "w":
                await GiveAllByType(player, ItemType.ITEM_WEAPON, amount, arg);
                break;

            case "relics":
            case "rel":
            case "r":
                await GiveAllByType(player, ItemType.ITEM_RELIQUARY, 1, arg);
                break;

            case "furniture":
            case "furn":
            case "f":
                await GiveAllByType(player, ItemType.ITEM_FURNITURE, amount, arg);
                break;

            case "essentials":
            case "ess":
                await GiveEssentials(player, amount, arg);
                break;

            case "all":
                await GiveAll(player, amount, arg);
                break;

            default:
                await arg.SendMsg(I18NManager.Translate("Game.Command.GiveAll.Usage"));
                break;
        }
    }

    #region Give All Avatars

    private static async ValueTask GiveAllAvatars(Game.Player.PlayerInstance player, CommandArg arg)
    {
        var addedAvatars = new List<AvatarDataInfo>();

        foreach (var avatarId in GameData.AvatarData.Keys)
        {
            if (IsExcludedAvatar(avatarId)) continue;
            if (player.AvatarManager.HasAvatar(avatarId)) continue;

            try
            {
                var avatar = await player.AddAvatar(avatarId, false);
                if (avatar == null) continue;

                player.AvatarManager.MaxOutAvatar(avatar);
                await player.SendPacket(new PacketAvatarAddNotify(avatar, false));
                addedAvatars.Add(avatar);
            }
            catch
            {
                // Skip invalid avatars (missing skillDepot, etc.)
            }
        }

        if (addedAvatars.Count > 0)
        {
            await player.SendPacket(new PacketAvatarDataNotify(player));
            await player.SendPacket(new PacketAvatarTeamAllDataNotify(player));
        }

        await arg.SendMsg(I18NManager.Translate("Game.Command.GiveAll.AvatarsGiven",
            addedAvatars.Count.ToString(), player.Uid.ToString()));
    }

    private static bool IsExcludedAvatar(int avatarId)
        => avatarId < 10000002
        || avatarId >= 11000000
        || (avatarId <= 10000910 && avatarId >= 10000900)
        || avatarId == GameConstants.MAIN_CHARACTER_MALE
        || avatarId == GameConstants.MAIN_CHARACTER_FEMALE;

    #endregion

    #region Give All By Type

    private static async ValueTask GiveAllByType(Game.Player.PlayerInstance player, ItemType targetType,
        int amount, CommandArg arg)
    {
        var items = new List<ItemData>();
        int created = 0;
        int addedTotal = 0;

        foreach (var (itemId, itemData) in GameData.ItemData)
        {
            if (itemData.ItemType != targetType) continue;
            if (itemData.UseOnGain) continue;

            if (targetType == ItemType.ITEM_WEAPON && !IsValidWeaponId(itemId))
                continue;
            if (targetType == ItemType.ITEM_RELIQUARY && !IsValidRelicId(itemId))
                continue;
            if (targetType == ItemType.ITEM_MATERIAL && itemId < MinMaterialId)
                continue;

            try
            {
                var item = new ItemData { ItemId = itemId };

                switch (targetType)
                {
                    case ItemType.ITEM_WEAPON:
                        item.Count = 1;
                        item.Level = Math.Max(1, amount);
                        item.PromoteLevel = ItemData.GetMinPromoteLevel(item.Level);
                        item.InitWeaponAffixes();
                        break;

                    case ItemType.ITEM_RELIQUARY:
                        item.Count = 1;
                        item.Level = 1;
                        break;

                    case ItemType.ITEM_MATERIAL:
                    case ItemType.ITEM_FURNITURE:
                        item.Count = Math.Min(amount, (int)itemData.StackLimit);
                        break;
                }

                items.Add(item);
                created++;
            }
            catch
            {
                // Skip items that fail to construct
            }

            if (items.Count >= ItemsPerBatch)
            {
                addedTotal += player.InventoryManager.AddItems(items, ActionReason.Gm);
                items.Clear();
            }
        }

        if (items.Count > 0)
            addedTotal += player.InventoryManager.AddItems(items, ActionReason.Gm);

        var i18nKey = targetType switch
        {
            ItemType.ITEM_WEAPON => "Game.Command.GiveAll.WeaponsGiven",
            ItemType.ITEM_RELIQUARY => "Game.Command.GiveAll.RelicsGiven",
            ItemType.ITEM_MATERIAL => "Game.Command.GiveAll.MaterialsGiven",
            ItemType.ITEM_FURNITURE => "Game.Command.GiveAll.FurnitureGiven",
            _ => "Game.Command.GiveAll.ItemsGiven"
        };

        // Report both created and actually-added counts for diagnosis
        if (addedTotal < created)
        {
            Logger.GetByClassName().Warn(
                $"giveall {targetType}: created {created}, only {addedTotal} added (capacity/validation rejected {created - addedTotal})");
        }

        await arg.SendMsg(I18NManager.Translate(i18nKey,
            addedTotal.ToString(), amount.ToString(), player.Uid.ToString()));
    }

    #endregion

    #region Give Essentials

    private static async ValueTask GiveEssentials(Game.Player.PlayerInstance player, int amount, CommandArg arg)
    {
        foreach (var itemId in Essentials)
            player.InventoryManager.AddItem(itemId, amount, ActionReason.Gm);

        await arg.SendMsg(I18NManager.Translate("Game.Command.GiveAll.EssentialsGiven",
            Essentials.Length.ToString(), amount.ToString(), player.Uid.ToString()));
    }

    #endregion

    #region Give All

    private static async ValueTask GiveAll(Game.Player.PlayerInstance player, int amount, CommandArg arg)
    {
        await GiveAllAvatars(player, arg);
        await GiveAllByType(player, ItemType.ITEM_WEAPON, Math.Max(1, amount), arg);
        await GiveAllByType(player, ItemType.ITEM_RELIQUARY, 1, arg);
        await GiveAllByType(player, ItemType.ITEM_FURNITURE, Math.Min(amount, 9999), arg);
        await GiveAllByType(player, ItemType.ITEM_MATERIAL, Math.Min(amount, 9999), arg);
        await GiveEssentials(player, Math.Min(amount, 9999), arg);

        await arg.SendMsg(I18NManager.Translate("Game.Command.GiveAll.AllGiven", player.Uid.ToString()));
    }

    #endregion
}

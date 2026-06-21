using System.Collections.Generic;
using System.Threading.Tasks;
using NahidaImpact.Data;
using NahidaImpact.Database.Avatar;
using NahidaImpact.Database.Inventory;
using NahidaImpact.Enums.Item;
using NahidaImpact.Enums.Player;
using NahidaImpact.GameServer.Server.Packet.Send.Avatar;
using NahidaImpact.GameServer.Server.Packet.Send.Team;
using NahidaImpact.Internationalization;
using NahidaImpact.Util;

namespace NahidaImpact.GameServer.Command.Commands;

[CommandInfo("give", "Game.Command.GiveAll.Desc", "Game.Command.GiveAll.Usage", ["g", "item"], [PermEnum.Admin])]
public class CommandGiveAll : ICommand
{
    private const int ItemsPerBatch = 200;

    private static bool IsValidWeaponId(int id) => id is >= 11101 and <= 19999;
    private static bool IsValidRelicId(int id) => id is >= 20002 and <= 29999;
    private const int MinMaterialId = 100_000;

    [CommandDefault]
    public static async ValueTask Execute(CommandArg arg)
    {
        if (!await arg.CheckTarget()) return;
        if (!await arg.CheckOnlineTarget()) return;

        var player = arg.Target!.Player!;
        var args = arg.Args;

        if (args.Count < 1)
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.GiveAll.Usage"));
            return;
        }

        // Parse tagged args: lv<level>, x<amount>, r<refinement>
        int lv = 1, amount = 1, refinement = 1;
        var positional = new List<string>();
        foreach (var a in args)
        {
            var lower = a.ToLower();
            if (lower.StartsWith("lv") && int.TryParse(a[2..], out var v)) lv = Math.Clamp(v, 1, 90);
            else if (lower.StartsWith('x') && int.TryParse(a[1..], out var c)) amount = Math.Max(1, c);
            else if (lower.StartsWith('r') && int.TryParse(a[1..], out var r)) refinement = Math.Clamp(r, 1, 5);
            else positional.Add(a);
        }

        if (positional.Count < 1)
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.GiveAll.Usage"));
            return;
        }

        var sub = positional[0].ToLower();

        switch (sub)
        {
            case "all":
                await GiveAllAvatars(player, arg);
                await GiveAllByType(player, ItemType.ITEM_WEAPON, amount, lv, refinement, arg);
                await GiveAllByType(player, ItemType.ITEM_RELIQUARY, 1, 1, 1, arg);
                await GiveAllByType(player, ItemType.ITEM_MATERIAL, amount, 1, 1, arg);
                await GiveAllByType(player, ItemType.ITEM_FURNITURE, amount, 1, 1, arg);
                await arg.SendMsg(I18NManager.Translate("Game.Command.GiveAll.AllGiven", player.Uid.ToString()));
                break;

            case "weapons":
            case "w":
                await GiveAllByType(player, ItemType.ITEM_WEAPON, amount, lv, refinement, arg);
                break;

            case "relics":
            case "reliquary":
            case "r":
                await GiveAllByType(player, ItemType.ITEM_RELIQUARY, 1, 1, 1, arg);
                break;

            case "mats":
            case "m":
                await GiveAllByType(player, ItemType.ITEM_MATERIAL, amount, 1, 1, arg);
                break;

            case "furniture":
            case "f":
                await GiveAllByType(player, ItemType.ITEM_FURNITURE, amount, 1, 1, arg);
                break;

            case "avatars":
            case "a":
                await GiveAllAvatars(player, arg);
                break;

            default:
                await arg.SendMsg(I18NManager.Translate("Game.Command.GiveAll.Usage"));
                break;
        }
    }

    #region Give All Avatars

    private static async ValueTask GiveAllAvatars(Game.Player.PlayerInstance player, CommandArg arg)
    {
        var added = new List<AvatarDataInfo>();

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
                added.Add(avatar);
            }
            catch { /* skip */ }
        }

        if (added.Count > 0)
        {
            await player.SendPacket(new PacketAvatarDataNotify(player));
            await player.SendPacket(new PacketAvatarTeamAllDataNotify(player));
        }

        await arg.SendMsg(I18NManager.Translate("Game.Command.GiveAll.AvatarsGiven",
            added.Count.ToString(), player.Uid.ToString()));
    }

    private static bool IsExcludedAvatar(int id)
        => id < 10000002
        || id >= 11000000
        || (id <= 10000910 && id >= 10000900)
        || id == GameConstants.MAIN_CHARACTER_MALE
        || id == GameConstants.MAIN_CHARACTER_FEMALE;

    #endregion

    #region Give All By Type

    private static async ValueTask GiveAllByType(Game.Player.PlayerInstance player, ItemType targetType,
        int amount, int level, int refinement, CommandArg arg)
    {
        var items = new List<ItemData>();
        int created = 0;

        foreach (var (itemId, itemData) in GameData.ItemData)
        {
            if (itemData.ItemType != targetType) continue;
            if (itemData.UseOnGain) continue;

            switch (targetType)
            {
                case ItemType.ITEM_WEAPON when !IsValidWeaponId(itemId): continue;
                case ItemType.ITEM_RELIQUARY when !IsValidRelicId(itemId): continue;
                case ItemType.ITEM_MATERIAL when itemId < MinMaterialId: continue;
            }

            try
            {
                var item = new ItemData { ItemId = itemId };

                if (targetType is ItemType.ITEM_WEAPON or ItemType.ITEM_RELIQUARY)
                {
                    item.Count = 1;
                    item.Level = targetType == ItemType.ITEM_WEAPON ? level : 1;
                    item.PromoteLevel = ItemData.GetMinPromoteLevel(item.Level);
                    if (targetType == ItemType.ITEM_WEAPON)
                    {
                        item.Refinement = refinement - 1;
                        item.InitWeaponAffixes();
                    }
                }
                else
                {
                    item.Count = Math.Min(amount, (int)itemData.StackLimit);
                }

                items.Add(item);
                created++;
            }
            catch { /* skip */ }

            if (items.Count >= ItemsPerBatch)
            {
                player.InventoryManager.AddItems(items, ActionReason.Gm);
                items.Clear();
            }
        }

        if (items.Count > 0)
            player.InventoryManager.AddItems(items, ActionReason.Gm);

        var key = targetType switch
        {
            ItemType.ITEM_WEAPON => "Game.Command.GiveAll.WeaponsGiven",
            ItemType.ITEM_RELIQUARY => "Game.Command.GiveAll.RelicsGiven",
            ItemType.ITEM_MATERIAL => "Game.Command.GiveAll.MaterialsGiven",
            ItemType.ITEM_FURNITURE => "Game.Command.GiveAll.FurnitureGiven",
            _ => "Game.Command.GiveAll.ItemsGiven"
        };

        await arg.SendMsg(I18NManager.Translate(key, created.ToString(), level.ToString(), refinement.ToString(), player.Uid.ToString()));
    }

    #endregion
}
using NahidaImpact.Data;
using NahidaImpact.Data.Excel;
using NahidaImpact.Database.Inventory;
using NahidaImpact.Enums;
using NahidaImpact.Enums.Item;
using NahidaImpact.Enums.Player;
using NahidaImpact.GameServer.Server.Packet.Send.Inventory;
using NahidaImpact.Internationalization;

namespace NahidaImpact.GameServer.Command.Commands;

[CommandInfo("giveall", "Game.Command.GiveAll.Desc", "Game.Command.GiveAll.Usage", ["ga"], [PermEnum.Admin])]
public class CommandGiveall : ICommands
{
    // Common Genshin material IDs for fast-add essentials
    private static readonly int[] Essentials =
    [
        201,  // Primogem
        202,  // Mora
        203,  // Genesis Crystal
        221,  // Intertwined Fate
        222,  // Acquaint Fate
        223,  // Starglitter
        224,  // Stardust
    ];

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
                await GiveAllAvatars(player, arg);
                break;

            case "mats":
            case "materials":
                await GiveAllByType(player, ItemType.ITEM_MATERIAL, amount, arg, "Game.Command.GiveAll.MaterialsGiven");
                break;

            case "weapons":
            case "wep":
                await GiveAllByType(player, ItemType.ITEM_WEAPON, 1, arg, "Game.Command.GiveAll.WeaponsGiven");
                break;

            case "relics":
            case "rel":
                await GiveAllByType(player, ItemType.ITEM_RELIQUARY, 1, arg, "Game.Command.GiveAll.RelicsGiven");
                break;

            case "essentials":
            case "ess":
                await GiveEssentials(player, amount, arg);
                break;

            case "all":
                await GiveAllAvatars(player, arg);
                await GiveAllByType(player, ItemType.ITEM_WEAPON, 1, arg, "Game.Command.GiveAll.WeaponsGiven");
                await GiveAllByType(player, ItemType.ITEM_RELIQUARY, 1, arg, "Game.Command.GiveAll.RelicsGiven");
                await GiveAllByType(player, ItemType.ITEM_MATERIAL, 9999, arg, "Game.Command.GiveAll.MaterialsGiven");
                await GiveEssentials(player, 9999, arg);
                break;

            default:
                await arg.SendMsg(I18NManager.Translate("Game.Command.GiveAll.Usage"));
                break;
        }
    }

    private static async ValueTask GiveAllAvatars(Game.Player.PlayerInstance player, CommandArg arg)
    {
        int added = 0;
        foreach (var avatarId in GameData.AvatarData.Keys)
        {
            if (player.AvatarManager.HasAvatar(avatarId)) continue;
            var result = await player.AddAvatar(avatarId, false);
            if (result != null) added++;
        }
        await arg.SendMsg(I18NManager.Translate("Game.Command.GiveAll.AvatarsGiven",
            added.ToString(), player.Uid.ToString()));
    }

    private static async ValueTask GiveAllByType(Game.Player.PlayerInstance player, ItemType targetType,
        int amount, CommandArg arg, string i18nKey)
    {
        var items = new List<ItemData>();
        int count = 0;

        foreach (var (itemId, itemData) in GameData.ItemData)
        {
            if (itemData.ItemType != targetType) continue;
            if (itemData.UseOnGain) continue; // Skip auto-use items (avatar cards, etc.)

            var item = new ItemData
            {
                ItemId = itemId,
                Count = targetType == ItemType.ITEM_MATERIAL
                    ? Math.Min(amount, (int)itemData.StackLimit)
                    : 1,
                Level = 1,
                Refinement = 1
            };

            // Set weapon affixes
            if (targetType == ItemType.ITEM_WEAPON && itemData.SkillAffix.Count > 0)
            {
                foreach (var affix in itemData.SkillAffix)
                {
                    if (affix > 0) item.Affixes.Add(affix);
                }
            }

            items.Add(item);
            count++;
        }

        if (items.Count > 0)
            player.InventoryManager.AddItems(items);

        // Send bulk hint
        if (items.Count > 0 && items.Count <= 100)
            _ = player.SendPacket(new PacketItemAddHintNotify(items, 0));

        await arg.SendMsg(I18NManager.Translate(i18nKey,
            count.ToString(), amount.ToString(), player.Uid.ToString()));
    }

    private static async ValueTask GiveEssentials(Game.Player.PlayerInstance player, int amount, CommandArg arg)
    {
        foreach (var itemId in Essentials)
        {
            player.InventoryManager.AddItem(itemId, amount);
        }
        await arg.SendMsg(I18NManager.Translate("Game.Command.GiveAll.EssentialsGiven",
            Essentials.Length.ToString(), amount.ToString(), player.Uid.ToString()));
    }
}

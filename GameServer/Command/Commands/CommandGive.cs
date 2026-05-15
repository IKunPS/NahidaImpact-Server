using NahidaImpact.Data;
using NahidaImpact.Data.Excel;
using NahidaImpact.Database.Inventory;
using NahidaImpact.Enums.Item;
using NahidaImpact.Enums.Player;
using NahidaImpact.GameServer.Server.Packet.Send.Inventory;
using NahidaImpact.Internationalization;

namespace NahidaImpact.GameServer.Command.Commands;

[CommandInfo("give", "Game.Command.Give.Desc", "Game.Command.Give.Usage", ["g", "item"], [PermEnum.Support, PermEnum.Admin])]
public class CommandGive : ICommands
{
    [CommandDefault]
    public static async ValueTask Execute(CommandArg arg)
    {
        if (arg.Args.Count < 1)
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.Give.Usage"));
            return;
        }

        if (!await arg.CheckTarget()) return;
        if (!await arg.CheckOnlineTarget()) return;

        var player = arg.Target!.Player!;

        if (!int.TryParse(arg.Args[0], out var itemId))
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.Give.InvalidItemId"));
            return;
        }

        int amount = arg.Args.Count >= 2 ? arg.GetInt(1) : 1;
        if (amount <= 0) amount = 1;
        int level = arg.Args.Count >= 3 ? arg.GetInt(2) : 1;
        if (level <= 0) level = 1;
        int refinement = arg.Args.Count >= 4 ? arg.GetInt(3) : 1;
        if (refinement <= 0) refinement = 1;

        if (!GameData.ItemData.TryGetValue(itemId, out var itemData))
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.Give.Failed", itemId.ToString()));
            return;
        }

        var item = CreateItem(itemId, amount, level, refinement, itemData, player);
        bool success = player.InventoryManager.AddItem(item);

        if (success)
        {
            _ = player.SendPacket(new PacketItemAddHintNotify(item, 0));
            await arg.SendMsg(I18NManager.Translate("Game.Command.Give.Success",
                amount.ToString(), itemId.ToString(), level.ToString(), player.Uid.ToString()));
        }
        else
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.Give.Failed", itemId.ToString()));
        }
    }

    private static ItemData CreateItem(int itemId, int amount, int level, int refinement, ItemDataExcel itemData, Game.Player.PlayerInstance player)
    {
        var item = new ItemData
        {
            ItemId = itemId,
            Count = amount,
            Level = level,
            Refinement = refinement
        };

        switch (itemData.ItemType)
        {
            case ItemType.ITEM_WEAPON:
                item.Count = 1;
                item.Level = Math.Max(1, level);
                // Set initial affixes from item data
                if (itemData.SkillAffix.Count > 0)
                {
                    foreach (var affix in itemData.SkillAffix)
                    {
                        if (affix > 0)
                            item.Affixes.Add(affix);
                    }
                }
                break;

            case ItemType.ITEM_RELIQUARY:
                item.Count = 1;
                item.Level = 1; // Relics always start at level 1
                if (itemData.AppendPropNum > 0)
                {
                    item.AppendPropIdList = new List<int>(new int[itemData.AppendPropNum]);
                }
                break;

            case ItemType.ITEM_MATERIAL:
            case ItemType.ITEM_FURNITURE:
                item.Count = Math.Min(amount, (int)itemData.StackLimit);
                item.Level = 1;
                break;

            case ItemType.ITEM_VIRTUAL:
                item.Level = 1;
                break;
        }

        return item;
    }
}

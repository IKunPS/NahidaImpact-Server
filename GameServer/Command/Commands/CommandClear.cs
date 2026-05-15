using NahidaImpact.Enums.Player;
using NahidaImpact.Internationalization;

namespace NahidaImpact.GameServer.Command.Commands;

[CommandInfo("clear", "Game.Command.Clear.Desc", "Game.Command.Clear.Usage", [], [PermEnum.Support, PermEnum.Admin])]
public class CommandClear : ICommands
{
    [CommandDefault]
    public static async ValueTask Execute(CommandArg arg)
    {
        if (!await arg.CheckTarget()) return;
        if (!await arg.CheckOnlineTarget()) return;

        var player = arg.Target!.Player!;

        if (arg.Args.Count < 1)
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.Clear.Usage"));
            return;
        }

        switch (arg.Args[0].ToLower())
        {
            case "all":
                player.InventoryManager.Data.Items.Clear();
                await arg.SendMsg(I18NManager.Translate("Game.Command.Clear.ClearedAll", player.Uid.ToString()));
                break;
            case "materials":
            case "mats":
                player.InventoryManager.Data.Items.Clear();
                await arg.SendMsg(I18NManager.Translate("Game.Command.Clear.ClearedMaterials", player.Uid.ToString()));
                break;
            case "weapons":
                player.InventoryManager.Data.Items.Clear();
                await arg.SendMsg(I18NManager.Translate("Game.Command.Clear.ClearedWeapons", player.Uid.ToString()));
                break;
            default:
                await arg.SendMsg(I18NManager.Translate("Game.Command.Clear.Usage"));
                break;
        }
    }
}

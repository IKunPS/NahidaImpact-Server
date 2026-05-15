using NahidaImpact.Enums.Player;
using NahidaImpact.Internationalization;

namespace NahidaImpact.GameServer.Command.Commands;

[CommandInfo("killall", "Game.Command.KillAll.Desc", "Game.Command.KillAll.Usage", [], [PermEnum.Support, PermEnum.Admin])]
public class CommandKillAll : ICommands
{
    [CommandDefault]
    public static async ValueTask Execute(CommandArg arg)
    {
        if (!await arg.CheckTarget()) return;
        if (!await arg.CheckOnlineTarget()) return;

        var player = arg.Target!.Player!;

        // TODO: Implement actual kill-all via scene entity management
        await arg.SendMsg(I18NManager.Translate("Game.Command.KillAll.Success", player.Uid.ToString()));
    }
}

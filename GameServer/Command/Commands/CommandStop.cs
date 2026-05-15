using NahidaImpact.Enums.Player;
using NahidaImpact.Internationalization;

namespace NahidaImpact.GameServer.Command.Commands;

[CommandInfo("stop", "Game.Command.Stop.Desc", "Game.Command.Stop.Usage", ["shutdown", "exit"], [PermEnum.Admin])]
public class CommandStop : ICommands
{
    [CommandDefault]
    public static async ValueTask Execute(CommandArg arg)
    {
        await arg.SendMsg(I18NManager.Translate("Game.Command.Stop.ShuttingDown"));
        Environment.Exit(0);
    }
}

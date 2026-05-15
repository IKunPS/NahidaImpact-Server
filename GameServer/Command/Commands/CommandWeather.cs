using NahidaImpact.Enums.Player;
using NahidaImpact.Internationalization;

namespace NahidaImpact.GameServer.Command.Commands;

[CommandInfo("weather", "Game.Command.Weather.Desc", "Game.Command.Weather.Usage", ["w"], [PermEnum.Support, PermEnum.Admin])]
public class CommandWeather : ICommands
{
    [CommandDefault]
    public static async ValueTask Execute(CommandArg arg)
    {
        if (!await arg.CheckTarget()) return;
        if (!await arg.CheckOnlineTarget()) return;

        var player = arg.Target!.Player!;

        if (arg.Args.Count < 1)
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.Weather.Usage"));
            return;
        }

        if (!int.TryParse(arg.Args[0], out var weatherType) || weatherType < 0 || weatherType > 5)
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.Weather.Invalid"));
            return;
        }

        // TODO: Implement actual weather change via scene packet
        await arg.SendMsg(I18NManager.Translate("Game.Command.Weather.Success", weatherType.ToString(), player.Uid.ToString()));
    }
}

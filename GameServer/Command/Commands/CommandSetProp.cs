using NahidaImpact.Enums.Player;
using NahidaImpact.Internationalization;

namespace NahidaImpact.GameServer.Command.Commands;

[CommandInfo("setprop", "Game.Command.SetProp.Desc", "Game.Command.SetProp.Usage", ["prop"], [PermEnum.Admin])]
public class CommandSetProp : ICommands
{
    [CommandDefault]
    public static async ValueTask Execute(CommandArg arg)
    {
        if (!await arg.CheckTarget()) return;
        if (!await arg.CheckOnlineTarget()) return;

        var player = arg.Target!.Player!;

        if (arg.Args.Count < 2)
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.SetProp.Usage"));
            return;
        }

        var prop = arg.Args[0].ToLower();
        var value = arg.Args[1];

        switch (prop)
        {
            case "worldlevel":
            case "wl":
                if (int.TryParse(value, out var wl))
                {
                    player.Data.WorldLevel = wl;
                    await arg.SendMsg(I18NManager.Translate("Game.Command.SetProp.WorldLevel", wl.ToString(), player.Uid.ToString()));
                }
                break;

            case "exp":
                if (int.TryParse(value, out var exp))
                {
                    player.Data.Exp = exp;
                    await arg.SendMsg(I18NManager.Translate("Game.Command.SetProp.Exp", exp.ToString(), player.Uid.ToString()));
                }
                break;

            case "name":
                player.Data.Name = value;
                player.Profile.Nickname = value;
                await arg.SendMsg(I18NManager.Translate("Game.Command.SetProp.Name", value, player.Uid.ToString()));
                break;

            case "signature":
                player.Data.Signature = value;
                await arg.SendMsg(I18NManager.Translate("Game.Command.SetProp.Signature", value, player.Uid.ToString()));
                break;

            default:
                await arg.SendMsg(I18NManager.Translate("Game.Command.SetProp.Unknown", prop));
                break;
        }
    }
}

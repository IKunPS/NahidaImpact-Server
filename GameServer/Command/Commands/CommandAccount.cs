using NahidaImpact.Database.Account;
using NahidaImpact.Enums.Player;
using NahidaImpact.Internationalization;

namespace NahidaImpact.GameServer.Command.Commands;

[CommandInfo("account", "Game.Command.Account.Desc", "Game.Command.Account.Usage", [], [PermEnum.Admin])]
public class CommandAccount : ICommands
{
    [CommandDefault]
    public static async ValueTask Execute(CommandArg arg)
    {
        if (arg.Args.Count < 1)
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.Account.Usage"));
            return;
        }

        switch (arg.Args[0].ToLower())
        {
            case "create":
                if (arg.Args.Count < 2)
                {
                    await arg.SendMsg(I18NManager.Translate("Game.Command.Account.Usage"));
                    return;
                }
                var username = arg.Args[1];
                int uid = arg.Args.Count >= 3 ? arg.GetInt(2) : 0;
                AccountData.CreateAccount(username, uid, "");
                await arg.SendMsg(I18NManager.Translate("Game.Command.Account.CreateSuccess", username, (uid > 0 ? uid.ToString() : "auto")));
                break;

            case "delete":
                if (arg.Args.Count < 2)
                {
                    await arg.SendMsg(I18NManager.Translate("Game.Command.Account.Usage"));
                    return;
                }
                int delUid = arg.GetInt(1);
                if (delUid <= 0)
                {
                    await arg.SendMsg(I18NManager.Translate("Game.Command.Account.InvalidUid"));
                    return;
                }
                AccountData.DeleteAccount(delUid);
                await arg.SendMsg(I18NManager.Translate("Game.Command.Account.DeleteSuccess", delUid.ToString()));
                break;

            default:
                await arg.SendMsg(I18NManager.Translate("Game.Command.Account.Usage"));
                break;
        }
    }
}

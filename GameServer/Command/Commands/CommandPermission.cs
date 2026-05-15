using NahidaImpact.Database.Account;
using NahidaImpact.Enums.Player;
using NahidaImpact.Internationalization;

namespace NahidaImpact.GameServer.Command.Commands;

[CommandInfo("permission", "Game.Command.Permission.Desc", "Game.Command.Permission.Usage", ["perm"], [PermEnum.Admin])]
public class CommandPermission : ICommands
{
    [CommandDefault]
    public static async ValueTask Execute(CommandArg arg)
    {
        if (!await arg.CheckTarget()) return;
        int targetUid = arg.TargetUid;

        if (arg.Args.Count < 1)
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.Permission.Usage"));
            return;
        }

        var action = arg.Args[0].ToLower();
        var account = AccountData.GetAccountByUid(targetUid);
        if (account == null)
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.Permission.AccountNotFound"));
            return;
        }

        switch (action)
        {
            case "add":
                if (arg.Args.Count < 2)
                {
                    await arg.SendMsg(I18NManager.Translate("Game.Command.Permission.Usage"));
                    return;
                }
                if (Enum.TryParse<PermEnum>(arg.Args[1], true, out var addPerm))
                {
                    AccountData.AddPerm([addPerm], targetUid);
                    await arg.SendMsg(I18NManager.Translate("Game.Command.Permission.Added", addPerm.ToString(), targetUid.ToString()));
                }
                else
                {
                    await arg.SendMsg(I18NManager.Translate("Game.Command.Permission.InvalidPerm", string.Join(", ", Enum.GetNames<PermEnum>())));
                }
                break;

            case "remove":
                if (arg.Args.Count < 2)
                {
                    await arg.SendMsg(I18NManager.Translate("Game.Command.Permission.Usage"));
                    return;
                }
                if (Enum.TryParse<PermEnum>(arg.Args[1], true, out var removePerm))
                {
                    AccountData.RemovePerm([removePerm], targetUid);
                    await arg.SendMsg(I18NManager.Translate("Game.Command.Permission.Removed", removePerm.ToString(), targetUid.ToString()));
                }
                else
                {
                    await arg.SendMsg(I18NManager.Translate("Game.Command.Permission.InvalidPerm", string.Join(", ", Enum.GetNames<PermEnum>())));
                }
                break;

            case "clear":
                AccountData.CleanPerm(targetUid);
                await arg.SendMsg(I18NManager.Translate("Game.Command.Permission.Cleared", targetUid.ToString()));
                break;

            case "list":
                var perms = account.Permissions ?? [];
                await arg.SendMsg(I18NManager.Translate("Game.Command.Permission.List", targetUid.ToString(), perms.Count > 0 ? string.Join(", ", perms) : "(none)"));
                break;

            default:
                await arg.SendMsg(I18NManager.Translate("Game.Command.Permission.Usage"));
                break;
        }
    }
}

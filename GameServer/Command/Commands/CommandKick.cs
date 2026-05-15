using NahidaImpact.Enums.Player;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.Internationalization;

namespace NahidaImpact.GameServer.Command.Commands;

[CommandInfo("kick", "Game.Command.Kick.Desc", "Game.Command.Kick.Usage", [], [PermEnum.Admin])]
public class CommandKick : ICommands
{
    [CommandDefault]
    public static async ValueTask Execute(CommandArg arg)
    {
        if (!await arg.CheckTarget()) return;

        var targetPlayer = PlayerInstance.GetPlayerInstanceByUid((uint)arg.TargetUid);
        if (targetPlayer == null)
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.Notice.PlayerNotFound"));
            return;
        }

        await targetPlayer.DropMessage(I18NManager.Translate("Game.Command.Kick.KickMessage"));
        targetPlayer.Connection?.Stop();
        await arg.SendMsg(I18NManager.Translate("Game.Command.Kick.PlayerKicked", arg.TargetUid.ToString()));
    }
}

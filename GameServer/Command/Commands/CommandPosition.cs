using NahidaImpact.Enums.Player;
using NahidaImpact.Internationalization;

namespace NahidaImpact.GameServer.Command.Commands;

[CommandInfo("position", "Game.Command.Position.Desc", "Game.Command.Position.Usage", ["pos"], [])]
public class CommandPosition : ICommands
{
    [CommandDefault]
    public static async ValueTask Execute(CommandArg arg)
    {
        if (!await arg.CheckTarget()) return;
        if (!await arg.CheckOnlineTarget()) return;

        var player = arg.Target!.Player!;
        var pos = player.Position;
        var rot = player.Rotation;

        await arg.SendMsg(I18NManager.Translate("Game.Command.Position.PlayerInfo", player.Uid.ToString(), player.SceneId.ToString()));
        await arg.SendMsg(I18NManager.Translate("Game.Command.Position.PositionInfo", pos.X.ToString("F1"), pos.Y.ToString("F1"), pos.Z.ToString("F1")));
        await arg.SendMsg(I18NManager.Translate("Game.Command.Position.RotationInfo", rot.X.ToString("F1"), rot.Y.ToString("F1"), rot.Z.ToString("F1")));
    }
}

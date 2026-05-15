using NahidaImpact.Enums.Player;
using NahidaImpact.GameServer.Game.Worlds;
using NahidaImpact.Internationalization;

namespace NahidaImpact.GameServer.Command.Commands;

[CommandInfo("teleport", "Game.Command.Teleport.Desc", "Game.Command.Teleport.Usage", ["tp"], [PermEnum.Support, PermEnum.Admin])]
public class CommandTeleport : ICommands
{
    [CommandDefault]
    public static async ValueTask Execute(CommandArg arg)
    {
        if (!await arg.CheckTarget()) return;
        if (!await arg.CheckOnlineTarget()) return;

        var player = arg.Target!.Player!;

        if (arg.Args.Count < 3)
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.Teleport.Usage"));
            return;
        }

        if (!float.TryParse(arg.Args[0], out var x) ||
            !float.TryParse(arg.Args[1], out var y) ||
            !float.TryParse(arg.Args[2], out var z))
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.Teleport.InvalidCoords"));
            return;
        }

        int sceneId = (int)player.SceneId;
        if (arg.Args.Count >= 4)
            _ = int.TryParse(arg.Args[3], out sceneId);

        var pos = new Position(x, y, z);
        bool result = player.World.TransferPlayerToScene(player, sceneId, pos);

        if (result)
            await arg.SendMsg(I18NManager.Translate("Game.Command.Teleport.Success", player.Data.Name ?? player.Uid.ToString(), x.ToString("F0"), y.ToString("F0"), z.ToString("F0"), sceneId.ToString()));
        else
            await arg.SendMsg(I18NManager.Translate("Game.Command.Teleport.Failed", sceneId.ToString()));
    }
}

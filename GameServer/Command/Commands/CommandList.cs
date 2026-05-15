using NahidaImpact.Enums.Player;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.Internationalization;

namespace NahidaImpact.GameServer.Command.Commands;

[CommandInfo("list", "Game.Command.List.Desc", "Game.Command.List.Usage", ["online", "players"], [PermEnum.Support, PermEnum.Admin])]
public class CommandList : ICommands
{
    [CommandDefault]
    public static async ValueTask Execute(CommandArg arg)
    {
        var players = PlayerInstance.PlayerInstances;
        await arg.SendMsg(I18NManager.Translate("Game.Command.List.Header", players.Count.ToString()));
        foreach (var p in players)
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.List.Entry",
                p.Uid.ToString(), p.Data.Name ?? "Unknown", p.SceneId.ToString(),
                p.Position.X.ToString("F0"), p.Position.Y.ToString("F0"), p.Position.Z.ToString("F0")));
        }
    }
}

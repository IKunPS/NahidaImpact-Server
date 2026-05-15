using NahidaImpact.Data;
using NahidaImpact.Enums.Player;
using NahidaImpact.Internationalization;

namespace NahidaImpact.GameServer.Command.Commands;

[CommandInfo("unlockall", "Game.Command.UnlockAll.Desc", "Game.Command.UnlockAll.Usage", ["ua"], [PermEnum.Support, PermEnum.Admin])]
public class CommandUnlockAll : ICommands
{
    [CommandDefault]
    public static async ValueTask Execute(CommandArg arg)
    {
        if (!await arg.CheckTarget()) return;
        if (!await arg.CheckOnlineTarget()) return;

        var player = arg.Target!.Player!;
        int totalPoints = 0;

        // Unlock all scene points for all loaded scenes (mirrors Java defaultUnlockAllMap)
        foreach (var (sceneId, scenePoints) in GameData.ScenePointsPerScene)
        {
            var unlocked = player.GetUnlockedScenePoints(sceneId);
            foreach (var pointId in scenePoints)
            {
                if (unlocked.Add(pointId))
                    totalPoints++;
            }
        }

        // Also unlock areas 1-999 for scenes 3-11
        var sceneAreas = Enumerable.Range(1, 999).ToHashSet();
        for (int i = 3; i <= 11; i++)
        {
            player.GetUnlockedSceneAreas(i).UnionWith(sceneAreas);
        }

        await arg.SendMsg(I18NManager.Translate("Game.Command.UnlockAll.Success", totalPoints.ToString(), player.Uid.ToString(), "3-11"));
    }
}

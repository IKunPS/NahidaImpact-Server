using NahidaImpact.Enums.Player;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.Internationalization;

namespace NahidaImpact.GameServer.Command.Commands;

[CommandInfo("heal", "Game.Command.Heal.Desc", "Game.Command.Heal.Usage", ["h"], [PermEnum.Support, PermEnum.Admin])]
public class CommandHeal : ICommands
{
    [CommandDefault]
    public static async ValueTask Execute(CommandArg arg)
    {
        if (!await arg.CheckTarget()) return;
        if (!await arg.CheckOnlineTarget()) return;

        var player = arg.Target!.Player!;
        var teamManager = player.TeamManager;
        if (teamManager == null)
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.Heal.NoTeam"));
            return;
        }

        var activeTeam = teamManager.GetActiveTeam();
        foreach (var entity in activeTeam)
        {
            if (entity is EntityAvatar avatar)
            {
                var maxHp = avatar.FightProperties.Find(f => (int)f.PropType == 3);
                var curHp = avatar.FightProperties.Find(f => (int)f.PropType == 2);
                if (maxHp != null && curHp != null)
                    curHp.PropValue = maxHp.PropValue;
            }
        }

        await arg.SendMsg(I18NManager.Translate("Game.Command.Heal.Success", player.Uid.ToString()));
    }
}

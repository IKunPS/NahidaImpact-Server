using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;
using NahidaImpact.Util;
using System.Threading.Tasks;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Ability;

[Opcode(CmdIds.ClientAbilityInitFinishNotify)]
public class HandlerClientAbilityInitFinishNotify : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        if (connection.Player == null) return;
        var notif = ClientAbilityInitFinishNotify.Parser.ParseFrom(data);
        var player = connection.Player;

        // Call skill end in the player's ability manager
        player.AbilityManager.OnSkillEnd(player);

        foreach (var entry in notif.Invokes)
        {
            player.AbilityManager.OnAbilityInvoke(entry);
            player.AbilityManager.ClientAbilityInitFinishHandler.AddEntry(entry);
        }

        if (notif.Invokes.Count > 0)
        {
            player.AbilityManager.ClientAbilityInitFinishHandler.Send();
        }

        await Task.CompletedTask;
    }
}

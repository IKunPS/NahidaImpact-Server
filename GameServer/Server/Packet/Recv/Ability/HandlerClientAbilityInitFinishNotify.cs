using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;
using System.Threading.Tasks;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Ability;

[Opcode(CmdIds.ClientAbilityInitFinishNotify)]
public class HandlerClientAbilityInitFinishNotify : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        if (connection.Player == null)
        {
            connection.Stop();
            return;
        }

        try
        {
            // TODO: Parse ClientAbilityInitFinishNotify when proto is available
            // For now, parse as AbilityInvocationsNotify (same invokes list structure)
            var notif = ClientAbilityInitFinishNotify.Parser.ParseFrom(data);

            var player = connection.Player;

            // Call skill end in the player's ability manager (mirrors Java)
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
        }
        catch { }

        await Task.CompletedTask;
    }
}

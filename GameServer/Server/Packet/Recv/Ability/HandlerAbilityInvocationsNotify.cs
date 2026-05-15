using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;
using System.Threading.Tasks;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Ability;

[Opcode(CmdIds.AbilityInvocationsNotify)]
public class HandlerAbilityInvocationsNotify : Handler
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
            var notify = AbilityInvocationsNotify.Parser.ParseFrom(data);
            foreach (var invoke in notify.Invokes)
            {
                connection.Player.AbilityManager.OnAbilityInvoke(invoke);
                connection.Player.AbilityManager.AbilityInvokeHandler.AddEntry(invoke);
            }
        }
        catch { }

        await Task.CompletedTask;
    }
}

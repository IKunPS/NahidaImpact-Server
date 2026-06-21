using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;
using System.Threading.Tasks;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Ability;

[Opcode(CmdIds.ClientAbilityChangeNotify)]
public class HandlerClientAbilityChangeNotify : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        if (connection.Player == null) return;
        var notify = ClientAbilityChangeNotify.Parser.ParseFrom(data);
        foreach (var invoke in notify.Invokes)
        {
            connection.Player.AbilityManager.OnAbilityInvoke(invoke);
            connection.Player.AbilityManager.AbilityInvokeHandler.AddEntry(invoke);
        }

        await Task.CompletedTask;
    }
}

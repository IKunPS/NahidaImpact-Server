using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Map;

[Opcode(CmdIds.ExitTransPointRegionNotify)]
public class HandlerExitTransPointRegionNotify : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var player = connection.Player!;
        player.StatueOfTheSevenManager?.HandleExitTransPointRegionNotify();

        await Task.CompletedTask;
    }
}
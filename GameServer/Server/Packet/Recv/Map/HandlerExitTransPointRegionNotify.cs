using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Map;

/// <summary>
/// Handles notification that player left a Statue of Seven region.
/// Ported from Java HandlerExitTransPointRegionNotify.
/// </summary>
[Opcode(CmdIds.ExitTransPointRegionNotify)]
public class HandlerExitTransPointRegionNotify : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var player = connection.Player!;
        player.SotSManager?.HandleExitTransPointRegionNotify();

        await Task.CompletedTask;
    }
}
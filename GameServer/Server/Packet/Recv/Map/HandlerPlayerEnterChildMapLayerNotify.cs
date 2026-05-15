using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Map;

[Opcode(CmdIds.PlayerEnterChildMapLayerNotify)]
public class HandlerPlayerEnterChildMapLayerNotify : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        // Client notified us it entered a child map layer - no action needed
        await Task.CompletedTask;
    }
}

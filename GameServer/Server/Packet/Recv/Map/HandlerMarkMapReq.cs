using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Map;

[Opcode(CmdIds.MarkMapReq)]
public class HandlerMarkMapReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        if (connection.Player == null) return;
        var req = MarkMapReq.Parser.ParseFrom(data);
        var player = connection.Player;

        player.MapMarksManager.HandleMapMarkReq(req);

        await Task.CompletedTask;
    }
}

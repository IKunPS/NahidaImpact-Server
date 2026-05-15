using NahidaImpact.GameServer.Server.Packet.Send.Map;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Map;

[Opcode(CmdIds.UnlockTransPointReq)]
public class HandlerUnlockTransPointReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = UnlockTransPointReq.Parser.ParseFrom(data);
        var player = connection.Player!;

        var success = player.ProgressManager.UnlockTransPoint(
            (int)req.SceneId, (int)req.PointId, false);

        await connection.SendPacket(new PacketUnlockTransPointRsp(success ? 0 : -1));
    }
}

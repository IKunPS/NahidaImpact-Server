using NahidaImpact.GameServer.Server.Packet.Send.Map;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Map;

[Opcode(CmdIds.GetScenePointReq)]
public class HandlerGetScenePointReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = GetScenePointReq.Parser.ParseFrom(data);
        var player = connection.Player!;

        await connection.SendPacket(new PacketGetScenePointRsp(player, (int)req.SceneId));
    }
}

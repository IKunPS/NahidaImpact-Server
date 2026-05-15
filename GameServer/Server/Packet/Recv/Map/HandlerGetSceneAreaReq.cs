using NahidaImpact.GameServer.Server.Packet.Send.Map;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Map;

[Opcode(CmdIds.GetSceneAreaReq)]
public class HandlerGetSceneAreaReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = GetSceneAreaReq.Parser.ParseFrom(data);
        var player = connection.Player!;

        await connection.SendPacket(new PacketGetSceneAreaRsp(player, (int)req.SceneId));
    }
}

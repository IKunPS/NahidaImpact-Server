using NahidaImpact.GameServer.Server.Packet.Send.Scene;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Scene;

[Opcode(CmdIds.EnterSceneReadyReq)]
public class HandlerEnterSceneReadyReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        await connection.SendPacket(new PacketEnterSceneReadyRsp(connection.Player!));
        await connection.SendPacket(new PacketEnterScenePeerNotify(connection.Player!));
    }
    
}
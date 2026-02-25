using NahidaImpact.GameServer.Server.Packet.Send.Scene;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Scene;

[Opcode(CmdIds.EnterSceneDoneReq)]
public class HandlerEnterSceneDoneReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        await connection.Player!.EntityManager!.AddEntityAsync(connection.Player!.EntityAvatar!, VisionType.VisionBorn);
        await connection.SendPacket(new PacketEnterSceneDoneRsp(connection.Player!));
    }

}
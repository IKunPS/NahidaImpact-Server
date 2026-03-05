using NahidaImpact.GameServer.Server.Packet.Send.Player;
using NahidaImpact.GameServer.Server.Packet.Send.Scene;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Scene;

[Opcode(CmdIds.SceneInitFinishReq)]
public class HandlerSceneInitFinishReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        await connection.SendPacket(new PacketPlayerEnterSceneInfoNotify(connection.Player!));
        await connection.SendPacket(new PacketSceneTeamUpdateNotify(connection.Player!));
        
        await connection.SendPacket(new PacketSceneInitFinishRsp(connection.Player!));
    }
    
}
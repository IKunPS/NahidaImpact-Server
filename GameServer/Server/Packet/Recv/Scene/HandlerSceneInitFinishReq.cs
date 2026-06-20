using NahidaImpact.Enums.Player;
using NahidaImpact.GameServer.Server.Packet.Send.Player;
using NahidaImpact.GameServer.Server.Packet.Send.Scene;
using NahidaImpact.GameServer.Server.Packet.Send.Time;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Scene;

[Opcode(CmdIds.SceneInitFinishReq)]
public class HandlerSceneInitFinishReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        if (connection.Player == null) return;
        var player = connection.Player;

        await connection.SendPacket(new PacketServerTimeNotify());
        await connection.SendPacket(new PacketSceneTimeNotify(player));
        await connection.SendPacket(new PacketPlayerEnterSceneInfoNotify(player));
        await connection.SendPacket(new PacketSceneTeamUpdateNotify(player));

        player.SceneLoadState = SceneLoadState.Init;
        await connection.SendPacket(new PacketSceneInitFinishRsp(player));
    }
}

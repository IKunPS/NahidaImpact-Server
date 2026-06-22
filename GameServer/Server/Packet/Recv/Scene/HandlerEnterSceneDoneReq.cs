using NahidaImpact.GameServer.Server.Packet.Send.Scene;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Scene;

[Opcode(CmdIds.EnterSceneDoneReq)]
public class HandlerEnterSceneDoneReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var player = connection.Player;
        if (player?.Scene == null) return;

        // Spawn player in world
        player.Scene.SpawnPlayer(player);

        // Show other entities already in the scene to this player.
        player.Scene.ShowOtherEntities(player);

        await connection.SendPacket(new PacketEnterSceneDoneRsp(player));
    }

}
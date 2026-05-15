using NahidaImpact.Data;
using NahidaImpact.GameServer.Game.Worlds;
using NahidaImpact.GameServer.Server.Packet.Send.Map;
using NahidaImpact.GameServer.Server.Packet.Send.Player;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Map;

[Opcode(CmdIds.PersonalSceneJumpReq)]
public class HandlerPersonalSceneJumpReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = PersonalSceneJumpReq.Parser.ParseFrom(data);
        var player = connection.Player!;
        var prevSceneId = (int)player.SceneId;
        var prevPos = player.Position.Clone();

        // Get the scene point entry from the current scene
        var entry = GameData.GetScenePointEntryById(prevSceneId, (int)req.PointId);

        if (entry != null && entry.PointData.TranPos != null)
        {
            var pos = new Position(
                entry.PointData.TranPos.X,
                entry.PointData.TranPos.Y,
                entry.PointData.TranPos.Z);
            int sceneId = entry.PointData.TranSceneId;

            player.World.TransferPlayerToScene(player, sceneId, pos);

            // Send scene enter notify for teleport
            await connection.SendPacket(new PacketPlayerEnterSceneNotify(player, prevSceneId, prevPos, sceneId, pos));

            player.PrevScene = prevSceneId;
            await connection.SendPacket(new PacketPersonalSceneJumpRsp(sceneId, pos));
        }
        else
        {
            // Fallback: send error response
            await connection.SendPacket(new PacketPersonalSceneJumpRsp(prevSceneId, player.Position));
        }
    }
}

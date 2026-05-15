using NahidaImpact.Data;
using NahidaImpact.GameServer.Game.Worlds;
using NahidaImpact.GameServer.Server.Packet.Send.Map;
using NahidaImpact.GameServer.Server.Packet.Send.Player;
using NahidaImpact.GameServer.Server.Packet.Send.Scene;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Map;

[Opcode(CmdIds.SceneTransToPointReq)]
public class HandlerSceneTransToPointReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = SceneTransToPointReq.Parser.ParseFrom(data);
        var player = connection.Player!;

        var entry = GameData.GetScenePointEntryById((int)req.SceneId, (int)req.PointId);

        if (entry != null && entry.PointData.TranPos != null)
        {
            var prevSceneId = (int)player.SceneId;
            var prevPos = player.Position.Clone();
            var tranPos = entry.PointData.TranPos;
            var targetPos = new Position(tranPos.X, tranPos.Y, tranPos.Z);

            player.World.TransferPlayerToScene(player, (int)req.SceneId, targetPos);

            // Send scene enter notify for teleport (mirrors Java teleport constructor)
            await connection.SendPacket(new PacketPlayerEnterSceneNotify(player, prevSceneId, prevPos, (int)req.SceneId, targetPos));

            if (player.EntityAvatar != null)
                player.Scene.BroadcastPacket(new PacketSceneEntityAppearNotify(player.EntityAvatar));
            await connection.SendPacket(new PacketSceneTransToPointRsp(player, (int)req.PointId, (int)req.SceneId));
        }
        else
        {
            await connection.SendPacket(new PacketSceneTransToPointRsp()); // Error response
        }
    }
}

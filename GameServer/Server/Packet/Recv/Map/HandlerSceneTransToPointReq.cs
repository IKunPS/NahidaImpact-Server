using NahidaImpact.Data;
using NahidaImpact.Enums.Player;
using NahidaImpact.GameServer.Game.Worlds;
using NahidaImpact.GameServer.Server.Packet.Send.Map;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Map;

[Opcode(CmdIds.SceneTransToPointReq)]
public class HandlerSceneTransToPointReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        if (connection.Player == null) return;
        var req = SceneTransToPointReq.Parser.ParseFrom(data);
        var player = connection.Player;

        // Resolve the teleport waypoint from game data.
        var entry = GameData.GetScenePointEntryById((int)req.SceneId, (int)req.PointId);

        if (entry?.PointData.TranPos != null)
        {
            var tranPos = entry.PointData.TranPos;
            var targetPos = new Position(tranPos.X, tranPos.Y, tranPos.Z);
            int sceneId = entry.PointData.TranSceneId != 0 ? entry.PointData.TranSceneId : (int)req.SceneId;

            // TransferPlayerToScene sends PlayerEnterSceneNotify internally.
            player.World.TransferPlayerToScene(player, sceneId, TeleportType.Waypoint, targetPos);

            await connection.SendPacket(new PacketSceneTransToPointRsp(player, (int)req.PointId, sceneId));
        }
        else
        {
            await connection.SendPacket(new PacketSceneTransToPointRsp());
        }
    }
}

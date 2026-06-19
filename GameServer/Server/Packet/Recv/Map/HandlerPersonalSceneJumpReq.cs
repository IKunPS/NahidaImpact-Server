using NahidaImpact.Data;
using NahidaImpact.Enums.Player;
using NahidaImpact.GameServer.Game.Worlds;
using NahidaImpact.GameServer.Server.Packet.Send.Map;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Map;

[Opcode(CmdIds.PersonalSceneJumpReq)]
public class HandlerPersonalSceneJumpReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = PersonalSceneJumpReq.Parser.ParseFrom(data);
        var player = connection.Player!;

        // Look up the scene point from the player's current scene.
        var entry = GameData.GetScenePointEntryById((int)player.SceneId, (int)req.PointId);

        if (entry?.PointData.TranPos != null)
        {
            var tranPos = entry.PointData.TranPos;
            var pos = new Position(tranPos.X, tranPos.Y, tranPos.Z);
            int sceneId = entry.PointData.TranSceneId != 0 ? entry.PointData.TranSceneId : (int)player.SceneId;

            // TransferPlayerToScene sends PlayerEnterSceneNotify internally.
            player.World.TransferPlayerToScene(player, sceneId, TeleportType.Internal, pos);

            await connection.SendPacket(new PacketPersonalSceneJumpRsp(sceneId, pos));
        }
        else
        {
            await connection.SendPacket(new PacketPersonalSceneJumpRsp((int)player.SceneId, player.Position));
        }
    }
}

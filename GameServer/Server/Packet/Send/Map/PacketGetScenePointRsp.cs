using NahidaImpact.Data;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Map;

public class PacketGetScenePointRsp : BasePacket
{
    public PacketGetScenePointRsp(PlayerInstance player, int sceneId, bool isRelogin = false, uint belongUid = 0) : base(CmdIds.GetScenePointRsp)
    {
        var proto = new GetScenePointRsp
        {
            SceneId = (uint)sceneId,
            BelongUid = belongUid,
            IsRelogin = isRelogin,
            Retcode = 0
        };

        List<uint> unlockedPoints;
        if (GameData.ScenePointIdList.Count == 0)
        {
            // Fallback: unlock all points 1..999
            unlockedPoints = Enumerable.Range(1, 999).Select(i => (uint)i).ToList();
        }
        else
        {
            unlockedPoints = player.GetUnlockedScenePoints(sceneId).Select(p => (uint)p).ToList();
        }

        // Core lists: unlocked + visible points
        proto.UnlockedPointList.AddRange(unlockedPoints);
        proto.UnhidePointList.AddRange(unlockedPoints);

        // Unlock areas 1..8
        for (int i = 1; i < 9; i++)
        {
            proto.UnlockAreaList.Add((uint)i);
        }

        SetData(proto);
    }
}

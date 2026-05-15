using NahidaImpact.GameServer.Game.MapMarks;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Map;

public class PacketMarkMapRsp : BasePacket
{
    public PacketMarkMapRsp(Dictionary<string, MapMark> mapMarks) : base(CmdIds.MarkMapRsp)
    {
        var proto = new MarkMapRsp
        {
            Retcode = 0
        };

        if (mapMarks != null)
        {
            foreach (var mapMark in mapMarks.Values)
            {
                if (mapMark.Position == null) continue;

                var markPoint = new MapMarkPoint
                {
                    SceneId = (uint)mapMark.SceneId,
                    Name = mapMark.Name ?? string.Empty,
                    Pos = new Vector
                    {
                        X = mapMark.Position.X,
                        Y = mapMark.Position.Y,
                        Z = mapMark.Position.Z
                    },
                    PointType = (MapMarkPointType)(int)mapMark.PointType,
                    MonsterId = (uint)mapMark.MonsterId,
                    FromType = (MapMarkFromType)(int)mapMark.FromType,
                    QuestId = (uint)mapMark.QuestId
                };

                proto.MarkList.Add(markPoint);
            }
        }

        SetData(proto);
    }
}

using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Map;

public class PacketMapAreaChangeNotify : BasePacket
{
    public PacketMapAreaChangeNotify(IEnumerable<MapAreaInfo> areas) : base(CmdIds.MapAreaChangeNotify)
    {
        var proto = new MapAreaChangeNotify();
        proto.MapAreaInfoList.AddRange(areas);

        SetData(proto);
    }
}

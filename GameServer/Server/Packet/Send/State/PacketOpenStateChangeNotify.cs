using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.State;

public class PacketOpenStateChangeNotify : BasePacket
{
    public PacketOpenStateChangeNotify(int openState, int value) : base(CmdIds.OpenStateChangeNotify)
    {
        var proto = new OpenStateChangeNotify();
        proto.OpenStateMap[(uint)openState] = (uint)value;
        SetData(proto);
    }

    public PacketOpenStateChangeNotify(Dictionary<int, int> stateMap) : base(CmdIds.OpenStateChangeNotify)
    {
        var proto = new OpenStateChangeNotify();
        foreach (var (key, val) in stateMap)
            proto.OpenStateMap[(uint)key] = (uint)val;
        SetData(proto);
    }
}

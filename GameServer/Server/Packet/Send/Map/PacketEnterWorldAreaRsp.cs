using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Map;

public class PacketEnterWorldAreaRsp : BasePacket
{
    public PacketEnterWorldAreaRsp(int areaId, int areaType) : base(CmdIds.EnterWorldAreaRsp)
    {
        var proto = new EnterWorldAreaRsp
        {
            AreaId = (uint)areaId,
            AreaType = (uint)areaType,
            Retcode = 0
        };
        SetData(proto);
    }
}
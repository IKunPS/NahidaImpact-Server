using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Map;

/// <summary>
/// Response packet for EnterWorldAreaReq.
/// Ported from Java PacketEnterWorldAreaRsp.
/// </summary>
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
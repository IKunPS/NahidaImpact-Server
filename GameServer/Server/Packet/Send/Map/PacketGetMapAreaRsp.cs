using NahidaImpact.Data;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Map;

public class PacketGetMapAreaRsp : BasePacket
{
    public PacketGetMapAreaRsp() : base(CmdIds.GetMapAreaRsp)
    {
        var proto = new GetMapAreaRsp();

        foreach (var mapAreaId in GameData.MapAreaData.Keys)
        {
            // Areas 61 and 64 are not open by default
            bool isOpen = mapAreaId != 64 && mapAreaId != 61;

            proto.MapAreaInfoList.Add(new MapAreaInfo
            {
                MapAreaId = (uint)mapAreaId,
                IsOpen = isOpen
            });
        }

        SetData(proto);
    }
}

using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Player;

public class PacketUpdatePlayerShowNameCardListRsp : BasePacket
{
    public PacketUpdatePlayerShowNameCardListRsp(IEnumerable<uint> showNameCardIdList) : base(CmdIds.UpdatePlayerShowNameCardListRsp)
    {
        var proto = new UpdatePlayerShowNameCardListRsp
        {
            Retcode = 0
        };
        proto.ShowNameCardIdList.AddRange(showNameCardIdList);
        SetData(proto);
    }
}

using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Player;

public class PacketGetAllUnlockNameCardRsp : BasePacket
{
    public PacketGetAllUnlockNameCardRsp(IEnumerable<uint> nameCardList) : base(CmdIds.GetAllUnlockNameCardRsp)
    {
        var proto = new GetAllUnlockNameCardRsp
        {
            Retcode = 0
        };
        proto.NameCardList.AddRange(nameCardList);
        SetData(proto);
    }
}

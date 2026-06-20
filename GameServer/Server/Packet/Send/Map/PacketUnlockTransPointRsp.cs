using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Map;

public class PacketUnlockTransPointRsp : BasePacket
{
    public PacketUnlockTransPointRsp(int retcode) : base(CmdIds.UnlockTransPointRsp)
    {
        var proto = new UnlockTransPointRsp
        {
            Retcode = retcode
        };

        SetData(proto);
    }
}

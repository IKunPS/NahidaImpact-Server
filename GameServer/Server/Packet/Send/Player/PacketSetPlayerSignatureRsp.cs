using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Player;

public class PacketSetPlayerSignatureRsp : BasePacket
{
    public PacketSetPlayerSignatureRsp(string signature) : base(CmdIds.SetPlayerSignatureRsp)
    {
        var proto = new SetPlayerSignatureRsp
        {
            Signature = signature,
            Retcode = 0
        };
        SetData(proto);
    }
}

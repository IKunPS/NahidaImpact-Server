using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Player;

public class PacketSetNameCardRsp : BasePacket
{
    public PacketSetNameCardRsp(bool success, uint nameCardId, uint fallbackNameCardId) : base(CmdIds.SetNameCardRsp)
    {
        var proto = new SetNameCardRsp
        {
            NameCardId = success ? nameCardId : fallbackNameCardId,
            Retcode = success ? 0 : 1
        };
        SetData(proto);
    }
}

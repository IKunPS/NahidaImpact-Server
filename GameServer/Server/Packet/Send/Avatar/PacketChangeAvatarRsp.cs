using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Avatar;

public class PacketChangeAvatarRsp : BasePacket
{
    public PacketChangeAvatarRsp(ulong guid) : base(CmdIds.ChangeAvatarRsp)
    {
        var proto = new ChangeAvatarRsp
        {
            CurGuid = guid
        };

        SetData(proto);
    }
}

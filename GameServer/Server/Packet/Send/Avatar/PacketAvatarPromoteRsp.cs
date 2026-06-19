using NahidaImpact.Database.Avatar;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Avatar;

public class PacketAvatarPromoteRsp : BasePacket
{
    public PacketAvatarPromoteRsp(AvatarDataInfo avatar)
        : base(CmdIds.AvatarPromoteRsp)
    {
        SetData(new AvatarPromoteRsp
        {
            Guid = avatar.Guid,
            Retcode = 0
        });
    }
}

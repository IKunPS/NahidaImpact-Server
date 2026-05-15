using NahidaImpact.Database.Avatar;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Avatar;

public class PacketAvatarAddNotify : BasePacket
{
    public PacketAvatarAddNotify(AvatarDataInfo avatar, bool isInTeam) : base(CmdIds.AvatarAddNotify)
    {
        var proto = new AvatarAddNotify
        {
            AvatarInfo = avatar.ToProto(),
            IsInTeam = isInTeam
        };

        SetData(proto);
    }
}

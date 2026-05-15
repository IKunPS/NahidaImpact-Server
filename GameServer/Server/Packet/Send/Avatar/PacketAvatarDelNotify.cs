using System.Collections.Generic;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Avatar;

public class PacketAvatarDelNotify : BasePacket
{
    public PacketAvatarDelNotify(List<ulong> avatarGuidList) : base(CmdIds.AvatarDelNotify)
    {
        var proto = new AvatarDelNotify();
        proto.AvatarGuidList.AddRange(avatarGuidList);

        SetData(proto);
    }
}

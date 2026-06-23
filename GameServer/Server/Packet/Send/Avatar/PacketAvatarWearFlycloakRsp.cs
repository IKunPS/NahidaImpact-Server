using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Avatar;

public class PacketAvatarWearFlycloakRsp : BasePacket
{
    public PacketAvatarWearFlycloakRsp(List<ulong> avatarGuidList, uint flycloakId, int retcode = 0)
        : base(CmdIds.AvatarWearFlycloakRsp)
    {
        var proto = new AvatarWearFlycloakRsp
        {
            FlycloakId = flycloakId,
            Retcode = retcode
        };
        proto.AvatarGuidList.AddRange(avatarGuidList);
        SetData(proto);
    }
}

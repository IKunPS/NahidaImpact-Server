using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Avatar;

public class PacketAvatarChangeCostumeRsp : BasePacket
{
    public PacketAvatarChangeCostumeRsp(ulong avatarGuid, uint costumeId)
        : base(CmdIds.AvatarChangeCostumeRsp)
    {
        var proto = new AvatarChangeCostumeRsp
        {
            AvatarGuid = avatarGuid,
            CostumeId = costumeId
        };
        SetData(proto);
    }

    public PacketAvatarChangeCostumeRsp()
        : base(CmdIds.AvatarChangeCostumeRsp)
    {
        SetData(new AvatarChangeCostumeRsp { Retcode = 1 });
    }
}

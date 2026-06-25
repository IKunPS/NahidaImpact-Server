using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Player;

public class PacketUpdatePlayerShowAvatarListRsp : BasePacket
{
    public PacketUpdatePlayerShowAvatarListRsp(IEnumerable<uint> showAvatarIdList, bool isShowAvatar) : base(CmdIds.UpdatePlayerShowAvatarListRsp)
    {
        var proto = new UpdatePlayerShowAvatarListRsp
        {
            IsShowAvatar = isShowAvatar,
            Retcode = 0
        };
        proto.ShowAvatarIdList.AddRange(showAvatarIdList);
        SetData(proto);
    }
}

using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Player;

public class PacketSetPlayerHeadImageRsp : BasePacket
{
    public PacketSetPlayerHeadImageRsp(ProfilePicture profilePicture) : base(CmdIds.SetPlayerHeadImageRsp)
    {
        var proto = new SetPlayerHeadImageRsp
        {
            ProfilePicture = profilePicture,
            Retcode = 0
        };
        SetData(proto);
    }
}

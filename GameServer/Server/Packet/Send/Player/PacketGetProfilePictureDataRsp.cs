using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Player;

public class PacketGetProfilePictureDataRsp : BasePacket
{
    public PacketGetProfilePictureDataRsp() : base(CmdIds.GetProfilePictureDataRsp) { }
}

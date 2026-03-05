using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Avatar;

public class PacketChangeTeamNameRsp : BasePacket
{
    public PacketChangeTeamNameRsp(int teamId, string teamName) : base(CmdIds.ChangeTeamNameRsp)
    {
        var proto = new ChangeTeamNameRsp
        {
            TeamId = teamId,
            TeamName = teamName
        };

        SetData(proto);
    }
}
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Team;

public class PacketChooseCurAvatarTeamRsp : BasePacket
{
    public PacketChooseCurAvatarTeamRsp(int teamId) : base(CmdIds.ChooseCurAvatarTeamRsp)
    {
        var proto = new ChooseCurAvatarTeamRsp
        {
            CurTeamId = (uint)teamId
        };

        SetData(proto);
    }
}

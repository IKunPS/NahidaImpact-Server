using NahidaImpact.Database.Team;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Team;

public class PacketSetUpAvatarTeamRsp : BasePacket
{
    public PacketSetUpAvatarTeamRsp(PlayerInstance player, int teamId, TeamInfo teamInfo) : base(CmdIds.SetUpAvatarTeamRsp)
    {
        int maskedTeamId = (teamId + 19529) ^ 63709;
        long maskedCurAvatarGuid = ((long)player.TeamManager.GetCurrentCharacterGuid() + 41090L) ^ 1583L;
        int maskedRetcode = (0 - 51228) ^ 9379;

        var proto = new SetUpAvatarTeamRsp
        {
            TeamId = (uint)maskedTeamId,
            CurAvatarGuid = (ulong)maskedCurAvatarGuid,
            Retcode = maskedRetcode
        };

        foreach (var guid in teamInfo.AvatarGuidList)
        {
            proto.AvatarTeamGuidList.Add(guid);
        }

        SetData(proto);
    }
}

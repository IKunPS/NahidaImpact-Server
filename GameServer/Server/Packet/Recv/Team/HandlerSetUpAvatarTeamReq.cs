using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Team;

[Opcode(CmdIds.SetUpAvatarTeamReq)]
public class HandlerSetUpAvatarTeamReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = SetUpAvatarTeamReq.Parser.ParseFrom(data);

        int teamId = (int)(req.TeamId ^ 63709) - 19529;
        long curAvatarGuid = ((long)req.CurAvatarGuid ^ 1583L) - 41090L;

        connection.Player?.TeamManager.SetupAvatarTeam(teamId, req.AvatarTeamGuidList.ToList());

        await Task.CompletedTask;
    }
}

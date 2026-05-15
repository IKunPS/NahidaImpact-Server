using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Team;

[Opcode(CmdIds.ChooseCurAvatarTeamReq)]
public class HandlerChooseCurAvatarTeamReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = ChooseCurAvatarTeamReq.Parser.ParseFrom(data);

        connection.Player?.TeamManager.SetCurrentTeam((int)req.TeamId);

        await Task.CompletedTask;
    }
}

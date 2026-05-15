using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Team;

[Opcode(CmdIds.ChangeTeamNameReq)]
public class HandlerChangeTeamNameReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = ChangeTeamNameReq.Parser.ParseFrom(data);

        connection.Player?.TeamManager.SetTeamName((int)req.TeamId, req.TeamName);

        await Task.CompletedTask;
    }
}

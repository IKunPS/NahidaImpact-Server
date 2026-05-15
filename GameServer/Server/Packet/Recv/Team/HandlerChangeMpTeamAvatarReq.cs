using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Team;

[Opcode(CmdIds.ChangeMpTeamAvatarReq)]
public class HandlerChangeMpTeamAvatarReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = ChangeMpTeamAvatarReq.Parser.ParseFrom(data);

        connection.Player?.TeamManager.SetupMpTeam(req.AvatarGuidList.ToList());

        await Task.CompletedTask;
    }
}

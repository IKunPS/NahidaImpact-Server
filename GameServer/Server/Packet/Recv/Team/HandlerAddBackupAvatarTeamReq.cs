using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Team;

[Opcode(CmdIds.AddBackupAvatarTeamReq)]
public class HandlerAddBackupAvatarTeamReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        connection.Player?.TeamManager.AddNewCustomTeam();

        await Task.CompletedTask;
    }
}

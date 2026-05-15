using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Team;

[Opcode(CmdIds.DelBackupAvatarTeamReq)]
public class HandlerDelBackupAvatarTeamReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = DelBackupAvatarTeamReq.Parser.ParseFrom(data);

        connection.Player?.TeamManager.RemoveCustomTeam((int)req.BackupAvatarTeamId);

        await Task.CompletedTask;
    }
}

using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Avatar;

[Opcode(CmdIds.AvatarSkillUpgradeReq)]
public class HandlerAvatarSkillUpgradeReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = AvatarSkillUpgradeReq.Parser.ParseFrom(data);
        if (connection.Player == null) return;
        await connection.Player.AvatarManager.UpgradeSkill(req.AvatarGuid, (int)req.AvatarSkillId);
    }
}

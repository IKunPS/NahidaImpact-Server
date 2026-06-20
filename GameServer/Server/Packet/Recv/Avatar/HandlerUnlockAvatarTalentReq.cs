using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Avatar;

[Opcode(CmdIds.UnlockAvatarTalentReq)]
public class HandlerUnlockAvatarTalentReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = UnlockAvatarTalentReq.Parser.ParseFrom(data);
        if (connection.Player == null) return;
        var avatar = connection.Player.AvatarManager.GetAvatarByGuid(req.AvatarGuid);
        if (avatar == null) return;
        await connection.Player.AvatarManager.UnlockConstellation(avatar, (int)req.TalentId);
    }
}

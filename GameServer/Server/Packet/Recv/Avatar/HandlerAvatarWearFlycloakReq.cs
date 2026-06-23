using NahidaImpact.GameServer.Server.Packet.Send.Avatar;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Avatar;

[Opcode(CmdIds.AvatarWearFlycloakReq)]
public class HandlerAvatarWearFlycloakReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var player = connection.Player;
        if (player == null) return;

        var req = AvatarWearFlycloakReq.Parser.ParseFrom(data);
        var flycloakId = req.FlycloakId;
        var firstGuid = req.AvatarGuidList.FirstOrDefault();

        // Validate ownership, change only the first avatar
        if (flycloakId != 0 && !player.FlyCloakList.Contains((int)flycloakId))
        {
            await connection.SendPacket(new PacketAvatarWearFlycloakRsp([], flycloakId, 1));
            return;
        }

        var avatar = player.AvatarManager.GetAvatarByGuid(firstGuid);
        if (avatar == null) return;

        avatar.WearingFlycloakId = flycloakId;
        player.AvatarManager.Save();

        // ChangeNotify first, then Rsp — both to player only
        await player.SendPacket(new PacketAvatarFlycloakChangeNotify([firstGuid], flycloakId));
        await connection.SendPacket(new PacketAvatarWearFlycloakRsp([firstGuid], flycloakId));
    }
}

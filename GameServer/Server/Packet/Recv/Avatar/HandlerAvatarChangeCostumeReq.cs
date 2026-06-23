using NahidaImpact.Data;
using NahidaImpact.Data.Excel;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.GameServer.Server.Packet.Send.Avatar;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Avatar;

[Opcode(CmdIds.AvatarChangeCostumeReq)]
public class HandlerAvatarChangeCostumeReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var player = connection.Player;
        if (player == null) return;

        var req = AvatarChangeCostumeReq.Parser.ParseFrom(data);
        var costumeId = req.CostumeId;
        var avatarGuid = req.AvatarGuid;

        var avatar = player.AvatarManager.GetAvatarByGuid(avatarGuid);
        if (avatar == null) return;

        // Only formal avatars (not trial) can change costume — mirrors Java FashionComp::wearCostume
        if (avatar.AvatarType != 1)
        {
            await connection.SendPacket(new PacketAvatarChangeCostumeRsp());
            return;
        }

        // Costume 0 = revert to default; non-zero must be owned and match the avatar's characterId
        if (costumeId != 0)
        {
            if (!player.CostumeList.Contains((int)costumeId))
            {
                await connection.SendPacket(new PacketAvatarChangeCostumeRsp());
                return;
            }

            if (!GameData.CostumeData.TryGetValue((int)costumeId, out var costumeData)
                || costumeData.CharacterId != avatar.AvatarId)
            {
                await connection.SendPacket(new PacketAvatarChangeCostumeRsp());
                return;
            }
        }

        avatar.CostumeId = costumeId;
        player.AvatarManager.Save();

        // Find the entity for this specific avatar (not just the current one) and broadcast
        var entity = FindAvatarEntity(player, avatarGuid);
        if (entity != null)
            player.Scene?.BroadcastPacket(new PacketAvatarChangeCostumeNotify(entity.ToProto()));

        await connection.SendPacket(new PacketAvatarChangeCostumeRsp(avatarGuid, costumeId));
    }

    private static EntityAvatar? FindAvatarEntity(PlayerInstance player, ulong avatarGuid)
    {
        var activeTeam = player.TeamManager?.GetActiveTeam();
        if (activeTeam == null) return null;
        foreach (var e in activeTeam)
        {
            if (e.AvatarInfo.Guid == avatarGuid)
                return e;
        }
        return null;
    }
}

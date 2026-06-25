using NahidaImpact.Data;
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

        // Only formal avatars (not trial) can change costume (hk4e FashionComp::wearCostume)
        if (avatar.AvatarType != 1)
        {
            await connection.SendPacket(new PacketAvatarChangeCostumeRsp());
            return;
        }

        // Costume 0 = revert to default; non-zero must be owned and belong to the avatar
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

        // Send Notify first, then Rsp — client expects both (hk4e FashionComp::notifyCostumeChange)
        var entity = FindAvatarEntityInScene(player, avatarGuid);
        if (entity != null)
        {
            player.Scene?.BroadcastPacket(new PacketAvatarChangeCostumeNotify(entity.ToProto()));
        }
        else
        {
            // Avatar not in scene — send minimal notify to the player so client stays in sync
            await player.SendPacket(new PacketAvatarChangeCostumeNotify(BuildOffFieldAvatarInfo(player, avatar)));
        }

        await connection.SendPacket(new PacketAvatarChangeCostumeRsp(avatarGuid, costumeId));
    }

    private static EntityAvatar? FindAvatarEntityInScene(PlayerInstance player, ulong avatarGuid)
    {
        if (player.Scene == null) return null;
        foreach (var entity in player.Scene.GetEntities())
        {
            if (entity is EntityAvatar avatarEntity && avatarEntity.AvatarInfo.Guid == avatarGuid)
                return avatarEntity;
        }
        return null;
    }

    private static SceneAvatarInfo BuildOffFieldAvatarInfo(PlayerInstance player, Database.Avatar.AvatarDataInfo avatar)
    {
        var info = new SceneAvatarInfo
        {
            Uid = (uint)player.Uid,
            AvatarId = avatar.AvatarId,
            Guid = avatar.Guid,
            PeerId = (uint)player.PeerId,
            SkillDepotId = avatar.SkillDepotId,
            CoreProudSkillLevel = avatar.CoreProudSkillLevel,
            WearingFlycloakId = avatar.WearingFlycloakId,
            BornTime = avatar.BornTime,
            CostumeId = avatar.CostumeId,
            WeaponSkinId = avatar.WeaponSkinId,
            TraceEffectId = avatar.TraceEffectId,
        };

        foreach (var talentId in avatar.TalentIdList)
            info.TalentIdList.Add(talentId);

        foreach (var kv in avatar.SkillLevelMap)
            info.SkillLevelMap[kv.Key] = kv.Value;

        foreach (var id in avatar.ProudSkillList)
            info.InherentProudSkillList.Add(id);

        foreach (var kv in avatar.ProudSkillExtraLevelMap)
            info.ProudSkillExtraLevelMap[kv.Key] = kv.Value;

        return info;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Scene;

public class PacketSceneTeamUpdateNotify : BasePacket
{
    public PacketSceneTeamUpdateNotify(PlayerInstance player) : base(CmdIds.SceneTeamUpdateNotify)
    {
        var proto = new SceneTeamUpdateNotify
        {
            IsInMp = player.IsInMultiplayer()
        };

        // Get current avatar entity for comparison (not used currently but kept for future)
        var currentAvatarEntity = player.TeamManager?.GetCurrentAvatarEntity();
        
        // Get active team - TeamManager should not be null but check anyway
        var activeTeam = player.TeamManager?.GetActiveTeam();
        if (activeTeam == null) 
        {
            SetData(proto);
            return;
        }
        
        foreach (var entityAvatar in activeTeam)
        {
            if (entityAvatar == null) continue;
            
            var sceneTeamAvatar = new SceneTeamAvatar
            {
                SceneEntityInfo = entityAvatar.ToProto(),
                WeaponEntityId = player.WeaponEntityId,
                PlayerUid = (uint)player.Uid,
                WeaponGuid = entityAvatar.AvatarInfo.WeaponGuid,
                EntityId = entityAvatar.Id,
                AvatarGuid = entityAvatar.AvatarInfo.Guid,
                AbilityControlBlock = entityAvatar.GetAbilityControlBlock(),
                SceneId = player.SceneId,
            };
            
            // For multiplayer, set additional fields like Java version
            if (player.IsInMultiplayer())
            {
                sceneTeamAvatar.AvatarInfo = entityAvatar.GetAvatarInfo();
                sceneTeamAvatar.SceneAvatarInfo = entityAvatar.GetSceneAvatarInfo();
            }
            
            proto.SceneTeamAvatarList.Add(sceneTeamAvatar);
        }

        SetData(proto);
    }
}
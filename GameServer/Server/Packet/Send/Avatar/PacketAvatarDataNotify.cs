using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;
using System.Collections.Generic;
using System.Linq;

namespace NahidaImpact.GameServer.Server.Packet.Send.Avatar;

public class PacketAvatarDataNotify : BasePacket
{
    public PacketAvatarDataNotify(PlayerInstance player) : base(CmdIds.AvatarDataNotify)
    {
        var proto = new AvatarDataNotify
        {
            CurAvatarTeamId = (uint)(player.TeamManager?.CurrentTeamIndex ?? 1),
            ChooseAvatarGuid = player.TeamManager?.GetCurrentCharacterGuid() ?? 0,
        };
        
        // Add owned flycloak, costume, trace effect lists                                                                                                                                                        
        proto.OwnedFlycloakList.AddRange(player.GetFlyCloakList().Select(x => (uint)x));                                                                                                                          
        proto.OwnedCostumeList.AddRange(player.GetCostumeList().Select(x => (uint)x));                                                                                                                            
        proto.OwnedTraceEffectList.AddRange(player.GetTraceEffectList().Select(x => (uint)x)); 

        // Avatar list
        foreach (var avatar in player.Avatars)
        {
            proto.AvatarList.Add(avatar.ToProto());
        }

        // Team data
        foreach (var kv in player.TeamManager?.Teams ?? new Dictionary<int, Database.Team.TeamInfo>())
        {
            proto.AvatarTeamMap[(uint)kv.Key] = kv.Value.ToProto();
            if (kv.Key > 4)
            {
                proto.BackupAvatarTeamOrderList.Add((uint)kv.Key);
            }
        }

        // Set main character as choose avatar if available
        var mainCharacter = player.AvatarManager.GetAvatar(player.GetMainCharacterId());
        if (mainCharacter != null)
        {
            proto.ChooseAvatarGuid = mainCharacter.Guid;
        }

        SetData(proto);
    }
}

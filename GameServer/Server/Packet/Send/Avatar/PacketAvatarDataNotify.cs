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
        
        proto.OwnedFlycloakList.AddRange(player.FlyCloakList.Select(x => (uint)x));                                                                                                                          
        proto.OwnedCostumeList.AddRange(player.CostumeList.Select(x => (uint)x));                                                                                                                            
        proto.OwnedTraceEffectList.AddRange(player.TraceEffectList.Select(x => (uint)x)); 

        foreach (var avatar in player.Avatars)
        {
            proto.AvatarList.Add(avatar.ToProto());
        }

        foreach (var kv in player.TeamManager?.Teams ?? new Dictionary<int, Database.Team.TeamInfo>())
        {
            proto.AvatarTeamMap[(uint)kv.Key] = kv.Value.ToProto();
            if (kv.Key > 4)
            {
                proto.BackupAvatarTeamOrderList.Add((uint)kv.Key);
            }
        }

        var mainCharacter = player.AvatarManager.GetAvatar(player.GetMainCharacterId());
        if (mainCharacter != null)
        {
            proto.ChooseAvatarGuid = mainCharacter.Guid;
        }

        SetData(proto);
    }
}
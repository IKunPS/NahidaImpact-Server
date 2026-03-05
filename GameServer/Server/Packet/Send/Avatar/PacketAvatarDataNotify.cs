using System;
using NahidaImpact.Database.Avatar;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.GameServer.Game.Player.Team;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;
using System.Collections.Generic;
using System.Linq;

namespace NahidaImpact.GameServer.Server.Packet.Send.Avatar;

public class PacketAvatarDataNotify : BasePacket
{
    public PacketAvatarDataNotify(PlayerInstance player, List<AvatarDataInfo> Avatars): base(CmdIds.AvatarDataNotify)
    {
        // Set choose avatar guid (similar to Java version)
        ulong chooseAvatarGuid = 228; // Default value like Java version
        var currentAvatarEntity = player.TeamManager?.GetCurrentAvatarEntity();
        if (currentAvatarEntity != null)
        {
            chooseAvatarGuid = currentAvatarEntity.AvatarInfo.Guid;
        }
        
        var proto = new AvatarDataNotify()
        {
            CurAvatarTeamId = player.TeamManager?.CurrentTeamIndex ?? 1,
            ChooseAvatarGuid = chooseAvatarGuid,
            AvatarList = { Avatars.Select(avatar => avatar.ToProto()) }
        };
        
        var teams = player.TeamManager?.GetAvatarTeams() ?? new List<GameAvatarTeam>();
        foreach (GameAvatarTeam team in teams)
        {
            AvatarTeam avatarTeam = new();
            avatarTeam.AvatarGuidList.AddRange(team.AvatarGuidList);

            proto.AvatarTeamMap.Add(team.Index, avatarTeam);
            if (team.Index > 4)
            {
                // Add to backup team order list (like Java version)
                proto.BackupAvatarTeamOrderList.Add((uint)team.Index);
            }
        }
        
        // Add owned flycloak list (default flycloak like Java version)
        // proto.OwnedFlycloakList.Add(340005); // Default wind glider
        
        // Add owned costume list (empty for now)
        // proto.OwnedCostumeList.Add(0);
        
        // Add owned trace effect list (empty for now)
        // proto.OwnedTraceEffectList.Add(0);
        
        // Add backup avatar team order list for teams 5-10 (like Java version)
        for (uint i = 5; i <= 10; i++)
        {
            if (!proto.BackupAvatarTeamOrderList.Contains(i))
            {
                proto.BackupAvatarTeamOrderList.Add(i);
            }
        }
        
        // Console.WriteLine($"[DEBUG] PacketAvatarDataNotify: OwnedFlycloakList.Count={proto.OwnedFlycloakList.Count}, BackupAvatarTeamOrderList.Count={proto.BackupAvatarTeamOrderList.Count}");
        
        SetData(proto);
    }
}
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Team;

public class PacketAvatarTeamAllDataNotify : BasePacket
{
    public PacketAvatarTeamAllDataNotify(PlayerInstance player) : base(CmdIds.AvatarTeamAllDataNotify)
    {
        var proto = new AvatarTeamAllDataNotify();
        var teamManager = player.TeamManager;

        // Add the id list for custom teams
        foreach (var id in teamManager.Teams.Keys)
        {
            if (id > 4)
            {
                proto.BackupAvatarTeamOrderList.Add((uint)id);
            }
        }

        // Add the avatar lists for all the teams the player has
        foreach (var kv in teamManager.Teams)
        {
            proto.AvatarTeamMap[(uint)kv.Key] = kv.Value.ToProto();
        }

        SetData(proto);
    }
}

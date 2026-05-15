using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Team;

public class PacketAvatarTeamUpdateNotify : BasePacket
{
    public PacketAvatarTeamUpdateNotify(PlayerInstance player) : base(CmdIds.AvatarTeamUpdateNotify)
    {
        var proto = new AvatarTeamUpdateNotify();
        var teamManager = player.TeamManager;

        if (teamManager.IsUsingTrialTeam())
        {
            foreach (var entity in teamManager.GetActiveTeam())
            {
                proto.TempAvatarGuidList.Add(entity.AvatarInfo.Guid);
            }
        }
        else
        {
            foreach (var kv in teamManager.Teams)
            {
                proto.AvatarTeamMap[(uint)kv.Key] = kv.Value.ToProto();
            }
        }

        SetData(proto);
    }

    /// <summary>
    /// Used for locking/unlocking team modification.
    /// </summary>
    public PacketAvatarTeamUpdateNotify() : base(CmdIds.AvatarTeamUpdateNotify)
    {
        SetData(new AvatarTeamUpdateNotify());
    }
}

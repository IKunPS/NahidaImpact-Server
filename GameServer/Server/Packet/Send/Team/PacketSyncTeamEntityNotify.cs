using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;
using NahidaImpact.Util;

namespace NahidaImpact.GameServer.Server.Packet.Send.Team;

public class PacketSyncTeamEntityNotify : BasePacket
{
    public PacketSyncTeamEntityNotify(PlayerInstance player) : base(CmdIds.SyncTeamEntityNotify)
    {
        // Phlogiston scalar value — mirrors Java:
        // AbilityScalarValueEntry with key "SGV_PlayerTeam_Phlogiston" and player's phlogiston value
        var scalarValue = new AbilityScalarValueEntry
        {
            FloatValue = player.GetPhlogistonValue()
        };
        scalarValue.Key = new AbilityString
        {
            Hash = Utils.AbilityHash("SGV_PlayerTeam_Phlogiston"),
            Str = "SGV_PlayerTeam_Phlogiston"
        };

        var phlogiston = new AbilitySyncStateInfo();
        phlogiston.SgvDynamicValueMap.Add(scalarValue);

        var proto = new SyncTeamEntityNotify
        {
            SceneId = player.SceneId
        };

        if (player.World?.IsMultiplayer() == true)
        {
            foreach (var p in player.World.GetPlayers())
            {
                if (player == p) continue;

                var entity = p.TeamManager?.Entity;
                if (entity == null) continue;

                var info = new TeamEntityInfo
                {
                    TeamEntityId = entity.Id,
                    AuthorityPeerId = p.PeerId,
                    TeamAbilityInfo = phlogiston
                };

                proto.TeamEntityInfoList.Add(info);
            }
        }

        SetData(proto);
    }
}

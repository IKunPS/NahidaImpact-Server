using System.Linq;
using NahidaImpact.Data;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;
using NahidaImpact.Util;

namespace NahidaImpact.GameServer.Server.Packet.Send.State;

public class PacketOpenStateUpdateNotify : BasePacket
{
    public PacketOpenStateUpdateNotify(PlayerInstance player) : base(CmdIds.OpenStateUpdateNotify)
    {
        var proto = new OpenStateUpdateNotify();

        foreach (var state in GameData.OpenStateData.Values)
        {
            uint id = state.Id;
            uint value = 0;

            if (ConfigManager.Config.GameOptions.EnabledOpenStateAllMap)
            {
                // Set all open states to 1 (same as /unlockall)
                value = 1;
                proto.OpenStateMap[id] = value;
                proto.OpenStateMap[48] = 1; // Enable map border
            }
            
            if (id == 45 && !ConfigManager.Config.GameOptions.ResinUsage)
            {
                // Remove resin from map
                proto.OpenStateMap[id] = 0;
                continue;
            }

            // If the player has an open state stored in their map, then it would always override any default value
            if (player.ProgressManager.OpenStates.TryGetValue((int)id, out int playerValue))
            {
                value = (uint)playerValue;
                proto.OpenStateMap[id] = value;
            }
            // Otherwise, add the state if it is contained in the set of default open states.
            else if (ProgressManager.DefaultOpenStates.Contains((int)id))
            {
                value = 1;
                proto.OpenStateMap[id] = value;
            }
        }

        SetData(proto);
    }
}
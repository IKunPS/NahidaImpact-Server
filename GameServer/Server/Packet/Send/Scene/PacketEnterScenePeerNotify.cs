using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Scene;

public class PacketEnterScenePeerNotify : BasePacket
{
    public PacketEnterScenePeerNotify(PlayerInstance player) : base(CmdIds.EnterScenePeerNotify)
    {
        var proto = new EnterScenePeerNotify()
        {
            DestSceneId = player.SceneId,
            HostPeerId = player.World?.GetHostPeerId() ?? (uint)player.PeerId,
            PeerId = (uint)player.PeerId,
            EnterSceneToken = player.EnterToken
        };
        
        SetData(proto);
    }
}
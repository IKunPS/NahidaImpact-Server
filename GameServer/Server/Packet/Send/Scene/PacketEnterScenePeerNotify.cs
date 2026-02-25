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
            HostPeerId = 1, // TODO: Scene peers
            PeerId = 1,
            EnterSceneToken = player.SceneManager.EnterToken
        };
        
        SetData(proto);
    }
}
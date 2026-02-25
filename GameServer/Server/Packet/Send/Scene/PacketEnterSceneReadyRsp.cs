using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Scene;

public class PacketEnterSceneReadyRsp : BasePacket
{
    public PacketEnterSceneReadyRsp(PlayerInstance player) : base(CmdIds.EnterSceneReadyRsp)
    {
        var proto = new EnterSceneReadyRsp()
        {
            EnterSceneToken = player.SceneManager.EnterToken
        };
        
        SetData(proto);
    }
}
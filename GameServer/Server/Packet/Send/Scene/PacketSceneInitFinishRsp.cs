using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Scene;

public class PacketSceneInitFinishRsp : BasePacket
{
    public PacketSceneInitFinishRsp(PlayerInstance player) : base(CmdIds.SceneInitFinishRsp)
    {
        var proto = new SceneInitFinishRsp()
        {
            EnterSceneToken = (player.SceneManager.EnterToken - 18277) ^ 51147
        };
        
        SetData(proto);
    }
}
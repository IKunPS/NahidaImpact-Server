using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Time;

public class PacketSceneTimeNotify : BasePacket
{
    public PacketSceneTimeNotify() : base(CmdIds.SceneTimeNotify)
    {
        var proto = new SceneTimeNotify
        {
            // TODO: 实现Java那边的逻辑
            IsPaused = false,
            SceneId = 3,
            SceneTime = (ulong)DateTimeOffset.Now.ToUnixTimeMilliseconds()
        };
        
        SetData(proto);
    }
}
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.KcpSharp;
using NahidaImpact.Prop;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Player;

public class PacketWorldPlayerInfoNotify : BasePacket
{
    public PacketWorldPlayerInfoNotify(PlayerInstance player) : base(CmdIds.WorldPlayerInfoNotify)
    {
        var proto = new WorldPlayerInfoNotify();
        proto.PlayerUidList.Add((uint)player.Uid);

        proto.PlayerInfoList.Add(new OnlinePlayerInfo
        {
            Uid = (uint)player.Uid,
            Nickname = player.Profile?.Nickname ?? "",
            PlayerLevel = (uint)player.GetProperty(PlayerProp.PROP_PLAYER_LEVEL),
            CurPlayerNumInWorld = 1
        });

        SetData(proto);
    }
}

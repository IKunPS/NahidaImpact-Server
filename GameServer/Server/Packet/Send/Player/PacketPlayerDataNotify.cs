using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.KcpSharp;
using NahidaImpact.Prop;
using NahidaImpact.Proto;
using NahidaImpact.Util;

namespace NahidaImpact.GameServer.Server.Packet.Send.Player;

public class PacketPlayerDataNotify : BasePacket
{
    public PacketPlayerDataNotify(PlayerInstance player) : base(CmdIds.PlayerDataNotify)
    {
        var proto = new PlayerDataNotify
        {
            NickName = player.Data.Name,
            ServerTime = (ulong)DateTimeOffset.Now.ToUnixTimeMilliseconds(),
            IsFirstLoginToday = true,
            RegionId = GameConstants.START_REGION_ID
        };

        foreach (var (key, value) in player.Properties)
        {
            proto.PropMap.Add(key, new PropValue { Type = key, Ival = value, Val = value });
        }

        SetData(proto);
    }
}
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.KcpSharp;
using NahidaImpact.Prop;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Player;

public class PacketPlayerDataNotify : BasePacket
{
    public PacketPlayerDataNotify(PlayerInstance player) : base(CmdIds.PlayerDataNotify)
    {
        var proto = new PlayerDataNotify
        {
            NickName = player.Data.Name,
            PropMap =
            {
                {PlayerProp.PROP_PLAYER_LEVEL, new() { Type = PlayerProp.PROP_PLAYER_LEVEL, Ival = 5 } },
                {PlayerProp.PROP_IS_FLYABLE, new() { Type = PlayerProp.PROP_IS_FLYABLE, Ival = 1 } },
                {PlayerProp.PROP_MAX_STAMINA, new() { Type = PlayerProp.PROP_MAX_STAMINA, Ival = 10000 } },
                {PlayerProp.PROP_CUR_PERSIST_STAMINA, new() { Type = PlayerProp.PROP_CUR_PERSIST_STAMINA, Ival = 10000 } },
                {PlayerProp.PROP_IS_TRANSFERABLE, new() { Type = PlayerProp.PROP_IS_TRANSFERABLE, Ival = 1 } },
                {PlayerProp.PROP_IS_SPRING_AUTO_USE, new() { Type = PlayerProp.PROP_IS_SPRING_AUTO_USE, Ival = 1 } },
                {PlayerProp.PROP_SPRING_AUTO_USE_PERCENT, new() { Type = PlayerProp.PROP_SPRING_AUTO_USE_PERCENT, Ival = 50 } }
            }
        };
        
        SetData(proto);
    }
}
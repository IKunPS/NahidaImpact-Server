using System;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Player;

public class PacketPlayerChatNotify : BasePacket
{
    public PacketPlayerChatNotify(PlayerInstance player, int channelId, int systemHintType) : base(CmdIds.PlayerChatNotify)
    {
        var systemHint = new ChatInfo.Types.SystemHint
        {
            Type = (uint)systemHintType
            // uid_list is optional, leave empty
        };
        
        var chatInfo = new ChatInfo
        {
            SystemHint = systemHint,
            Uid = (uint)player.Uid,
            Time = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            PlatformType = 0, // Default platform type
            IsRead = false,
            Sequence = 1 // Default sequence
        };
        
        var proto = new PlayerChatNotify
        {
            ChatInfo = chatInfo,
            ChannelId = (uint)channelId
        };
        
        SetData(proto);
    }
}
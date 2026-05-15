using System;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Player;

public class PacketPlayerChatNotify : BasePacket
{
    // Text message
    public PacketPlayerChatNotify(PlayerInstance player, int channelId, string message) : base(CmdIds.PlayerChatNotify)
    {
        var chatInfo = new ChatInfo
        {
            Uid = (uint)player.Uid,
            Text = message,
            Time = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        var proto = new PlayerChatNotify
        {
            ChatInfo = chatInfo,
            ChannelId = (uint)channelId
        };

        SetData(proto);
    }

    // Icon/emote message
    public PacketPlayerChatNotify(PlayerInstance player, int channelId, int icon) : base(CmdIds.PlayerChatNotify)
    {
        var chatInfo = new ChatInfo
        {
            Uid = (uint)player.Uid,
            Icon = (uint)icon,
            Time = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        var proto = new PlayerChatNotify
        {
            ChatInfo = chatInfo,
            ChannelId = (uint)channelId
        };

        SetData(proto);
    }

    // System hint (enter/leave world)
    public PacketPlayerChatNotify(PlayerInstance player, int channelId, ChatInfo.Types.SystemHint systemHint) : base(CmdIds.PlayerChatNotify)
    {
        var chatInfo = new ChatInfo
        {
            SystemHint = systemHint,
            Uid = (uint)player.Uid,
            Time = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        var proto = new PlayerChatNotify
        {
            ChatInfo = chatInfo,
            ChannelId = (uint)channelId
        };

        SetData(proto);
    }
}
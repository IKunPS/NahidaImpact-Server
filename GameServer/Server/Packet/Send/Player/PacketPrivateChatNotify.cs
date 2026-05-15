using System;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Player;

/// <summary>
/// Sends a private chat message between two UIDs via the chat system.
/// </summary>
public class PacketPrivateChatNotify : BasePacket
{
    public ChatInfo ChatInfo { get; }

    public PacketPrivateChatNotify(int fromUid, int toUid, string message) : base(CmdIds.PrivateChatNotify)
    {
        ChatInfo = new ChatInfo
        {
            Uid = (uint)fromUid,
            ToUid = (uint)toUid,
            Text = message,
            Time = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            IsRead = false
        };

        var proto = new PrivateChatNotify
        {
            ChatInfo = ChatInfo
        };

        SetData(proto);
    }

    public PacketPrivateChatNotify(int fromUid, int toUid, int emote) : base(CmdIds.PrivateChatNotify)
    {
        ChatInfo = new ChatInfo
        {
            Uid = (uint)fromUid,
            ToUid = (uint)toUid,
            Icon = (uint)emote,
            Time = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        var proto = new PrivateChatNotify
        {
            ChatInfo = ChatInfo
        };

        SetData(proto);
    }
}

using System.Collections.Generic;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Player;

public class PacketPullRecentChatRsp : BasePacket
{
    public PacketPullRecentChatRsp(List<ChatInfo> messages) : base(CmdIds.PullRecentChatRsp)
    {
        var proto = new PullRecentChatRsp();
        if (messages != null)
        {
            proto.ChatInfo.AddRange(messages);
        }
        SetData(proto);
    }
}
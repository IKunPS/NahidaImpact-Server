using System.Collections.Generic;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Player;

public class PacketPullPrivateChatRsp : BasePacket
{
    public PacketPullPrivateChatRsp(List<ChatInfo> history) : base(CmdIds.PullPrivateChatRsp)
    {
        var proto = new PullPrivateChatRsp();
        if (history != null)
        {
            proto.ChatInfo.AddRange(history);
        }
        else
        {
            proto.Retcode = (int)Retcode.RetFail;
        }
        SetData(proto);
    }
}
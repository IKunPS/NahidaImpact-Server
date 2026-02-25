using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.State;

public class PacketOpenStateUpdateNotify : BasePacket
{
    public PacketOpenStateUpdateNotify() : base(CmdIds.OpenStateUpdateNotify)
    {
        var proto = new OpenStateUpdateNotify()
        {
            OpenStateMap =
            {
                {1, 1},
                {2, 1},
                {3, 1},
                {4, 1},
                {5, 1},
                {6, 1},
                {7, 0},
                {8, 1},
                {10, 1},
                {11, 1},
                {12, 1},
                {13, 1},
                {14, 1},
                {15, 1},
                {27, 1},
                {28, 1},
                {29, 1},
                {30, 1},
                {31, 1},
                {32, 1},
                {33, 1},
                {37, 1},
                {38, 1},
                {45, 1},
                {47, 1},
                {53, 1},
                {54, 1},
                {55, 1},
                {59, 1},
                {62, 1},
                {65, 1},
                {900, 1},
                {901, 1},
                {902, 1},
                {903, 1},
                {1001, 1},
                {1002, 1},
                {1003, 1},
                {1004, 1},
                {1005, 1},
                {1007, 1},
                {1008, 1},
                {1009, 1},
                {1010, 1},
                {1100, 1},
                {1103, 1},
                {1300, 1},
                {1401, 1},
                {1403, 1},
                {1700, 1},
                {2100, 1},
                {2101, 1},
                {2103, 1},
                {2400, 1},
                {3701, 1},
                {3702, 1},
                {4100, 1 } 
            }
        };
        
        SetData(proto);
    }
}
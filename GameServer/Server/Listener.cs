using NahidaImpact.KcpSharp;

namespace NahidaImpact.GameServer.Server;

public class Listener : KcpListener
{
    public static Connection? GetActiveConnection(int uid)
    {
        var con = Connections.Values.FirstOrDefault(c =>
            (c as Connection)?.Player?.Uid == uid && c.State == SessionStateEnum.ACTIVE) as Connection;
        return con;
    }
}
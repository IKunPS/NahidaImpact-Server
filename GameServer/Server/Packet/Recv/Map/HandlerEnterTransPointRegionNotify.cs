using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Map;

/// <summary>
/// Handles notification that player entered a Statue of Seven region.
/// Ported from Java HandlerEnterTransPointRegionNotify.
/// </summary>
[Opcode(CmdIds.EnterTransPointRegionNotify)]
public class HandlerEnterTransPointRegionNotify : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var player = connection.Player!;
        var notify = EnterTransPointRegionNotify.Parser.ParseFrom(data);

        // Delegate to SotSManager for auto-revive and healing
        player.SotSManager?.HandleEnterTransPointRegionNotify((int)notify.SceneId, (int)notify.PointId);

        await Task.CompletedTask;
    }
}
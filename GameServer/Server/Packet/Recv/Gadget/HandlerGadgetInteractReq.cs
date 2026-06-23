using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Gadget;

[Opcode(CmdIds.GadgetInteractReq)]
public class HandlerGadgetInteractReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = GadgetInteractReq.Parser.ParseFrom(data);
        var player = connection.Player;
        if (player?.Scene == null) return;

        var entity = player.Scene.GetEntityById((int)req.GadgetEntityId);
        if (entity is not EntityGadget gadget) return;

        gadget.OnInteract(player, req);

        var rsp = new BasePacket((ushort)(CmdIds.GadgetInteractReq + 1));
        rsp.SetData(new GadgetInteractRsp
        {
            Retcode = 0,
            GadgetId = req.GadgetId,
            GadgetEntityId = req.GadgetEntityId,
            OpType = req.OpType,
            InteractType = InteractType.InteractNone,
        });
        await player.SendPacket(rsp);
    }
}

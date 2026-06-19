using NahidaImpact.GameServer.Game.Player;

namespace NahidaImpact.GameServer.Game.Entity.Gadget;

public abstract class GadgetContent
{
    private readonly EntityGadget _gadget;

    public GadgetContent(EntityGadget gadget)
    {
        _gadget = gadget;
    }

    public EntityGadget GetGadget()
    {
        return _gadget;
    }

    public abstract bool OnInteract(PlayerInstance player, GadgetInteractReq req);
    public abstract void OnBuildProto(SceneGadgetInfo gadgetInfo);
}
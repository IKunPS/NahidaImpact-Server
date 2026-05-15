using NahidaImpact.GameServer.Game.Player;

namespace NahidaImpact.GameServer.Game.Entity.Gadget;

public abstract class GadgetContent
{
    private readonly EntityGadget gadget;
    
    public GadgetContent(EntityGadget gadget) {
        this.gadget = gadget;
    }
    
    public EntityGadget getGadget() {
        return gadget;
    }
    
    public abstract bool onInteract(PlayerInstance player, GadgetInteractReq req);
    public abstract void onBuildProto(SceneGadgetInfo gadgetInfo);
}
using NahidaImpact.GameServer.Game.Player;

namespace NahidaImpact.GameServer.Game.Entity.Gadget;

// hk4e GadgetBaseInteractComp — base interaction component for all gadget types
public abstract class GadgetContent
{
    protected readonly EntityGadget Gadget;

    protected GadgetContent(EntityGadget gadget)
    {
        Gadget = gadget;
    }

    public abstract bool OnInteract(PlayerInstance player, GadgetInteractReq req);
    public abstract void OnBuildProto(SceneGadgetInfo info);

    public virtual void OnStateChanged(uint oldState, uint newState) { }
}

// hk4e ChestComp — treasure chest
public class ChestGadgetContent : GadgetContent
{
    public uint ChestDropId { get; set; }
    public bool IsOpened => Gadget.GadgetState == 1;

    public ChestGadgetContent(EntityGadget gadget) : base(gadget) { }

    public override bool OnInteract(PlayerInstance player, GadgetInteractReq req)
    {
        if (IsOpened) return false;
        Gadget.UpdateState(1);
        Gadget.SetEnableInteract(false);
        // TODO: grant drops via DropManager using ChestDropId
        return true;
    }

    public override void OnBuildProto(SceneGadgetInfo info)
    {
        info.TrifleGadget = new TrifleGadget();
    }
}

// hk4e WorktopComp — crafting table / alchemy table
public class WorktopGadgetContent : GadgetContent
{
    public List<uint> RecipeIds { get; } = [];

    public WorktopGadgetContent(EntityGadget gadget) : base(gadget) { }

    public override bool OnInteract(PlayerInstance player, GadgetInteractReq req) => true;

    public override void OnBuildProto(SceneGadgetInfo info)
    {
        var worktop = new WorktopInfo();
        info.Worktop = worktop;
    }
}

// hk4e CrystalComp — mining crystal
public class CrystalGadgetContent : GadgetContent
{
    public int ResinCost { get; set; }
    public List<uint> DropIds { get; } = [];

    public CrystalGadgetContent(EntityGadget gadget) : base(gadget) { }

    public override bool OnInteract(PlayerInstance player, GadgetInteractReq req)
    {
        if (Gadget.GadgetState > 0) return false;
        Gadget.UpdateState(1);
        return true;
    }

    public override void OnBuildProto(SceneGadgetInfo info) { }
}

// hk4e GatherPointComp — open-world gathering node
public class GatherPointGadgetContent : GadgetContent
{
    public int GatherItemId { get; set; }

    public GatherPointGadgetContent(EntityGadget gadget) : base(gadget) { }

    public override bool OnInteract(PlayerInstance player, GadgetInteractReq req)
    {
        if (Gadget.GadgetState > 0) return false;
        Gadget.UpdateState(1);
        return true;
    }

    public override void OnBuildProto(SceneGadgetInfo info)
    {
        info.GatherGadget = new GatherGadgetInfo { ItemId = (uint)GatherItemId, IsForbidGuest = false };
    }
}

// hk4e StatueComp — Statue of the Seven
public class StatueGadgetContent : GadgetContent
{
    public StatueGadgetContent(EntityGadget gadget) : base(gadget) { }

    public override bool OnInteract(PlayerInstance player, GadgetInteractReq req) => true;

    public override void OnBuildProto(SceneGadgetInfo info)
    {
        info.StatueGadget = new StatueGadgetInfo();
    }
}

// hk4e FoundationComp — building foundation (teapot)
public class FoundationGadgetContent : GadgetContent
{
    public FoundationGadgetContent(EntityGadget gadget) : base(gadget) { }

    public override bool OnInteract(PlayerInstance player, GadgetInteractReq req) => true;

    public override void OnBuildProto(SceneGadgetInfo info)
    {
        info.FoundationInfo = new FoundationInfo();
    }
}

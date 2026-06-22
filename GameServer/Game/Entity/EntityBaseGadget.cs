using NahidaImpact.Enums.Entity;
using NahidaImpact.GameServer.Game.Worlds;

namespace NahidaImpact.GameServer.Game.Entity;

public abstract class EntityBaseGadget : BaseEntity
{
    protected readonly Position position;
    protected readonly Position rotation;
    private readonly int campId;
    private readonly int campType;

    public EntityBaseGadget(Scene scene, Position? position = null, Position? rotation = null, int campId = 0, int campType = 0) : base(scene)
    {
        this.position = position?.Clone() ?? new Position();
        this.rotation = rotation?.Clone() ?? new Position();
        this.campId = campId;
        this.campType = campType;
    }

    public override Position Position => position;
    public override Position Rotation => rotation;

    public override uint EntityTypeId => (uint)EntityIdTypeEnum.Gadget;
}
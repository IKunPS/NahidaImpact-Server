using NahidaImpact.Enums.Entity;
using NahidaImpact.GameServer.Game.Worlds;

namespace NahidaImpact.GameServer.Game.Entity;

public abstract class EntityBaseGadget : BaseEntity
{
    protected readonly Position position;
    protected readonly Position rotation;
    private readonly int campId;
    private readonly int campType;
    
    public EntityBaseGadget(Scene scene) : this(scene, null, null) {
    }
    
    public EntityBaseGadget(Scene scene, Position position, Position rotation) : this(scene, position, rotation, 0, 0) {
        
    }
    
    public EntityBaseGadget(
        Scene scene, Position position, Position rotation, int campId, int campType) : base(scene) {
        this.position = position != null ? position.Clone() : new Position();
        this.rotation = rotation != null ? rotation.Clone() : new Position();
        this.campId = campId;
        this.campType = campType;
    }
    
    public override Position GetPosition() => position;
    public override Position GetRotation() => rotation;

    public override uint getEntityTypeId()
    {
        return (uint)EntityIdTypeEnum.Gadget;
    }
}

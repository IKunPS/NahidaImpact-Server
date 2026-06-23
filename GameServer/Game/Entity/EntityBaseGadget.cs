using NahidaImpact.Enums.Entity;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.GameServer.Game.Worlds;

namespace NahidaImpact.GameServer.Game.Entity;

// hk4e Gadget — inherits Creature, uses EcsBase for component system (ability, client, interaction)
public abstract class EntityBaseGadget : BaseEntity
{
    protected readonly Position _position;
    protected readonly Position _rotation;

    // hk4e Gadget core fields
    public int GadgetId { get; protected set; }
    public uint MasterUid { get; set; }
    public uint DropId { get; set; }
    public bool IsBanDrop { get; set; }
    public uint StateBeginTime { get; set; }
    public long CreateTimeMs { get; set; }
    public bool IsByQuest { get; set; }
    public uint GadgetTalkState { get; set; }
    public uint GadgetState { get; set; }
    public GadgetBornType BornType { get; set; } = GadgetBornType.None;
    public bool IsEnableInteract { get; set; } = true;
    public uint GuestBanDrop { get; set; }
    public uint PointConfigId { get; set; }

    // hk4e weak_ptr references — simplified to direct references in C#
    public PlayerInstance? PropOwner { get; set; }
    public EntityBaseGadget? FoundationGadget { get; set; }
    public List<uint> InteractUids { get; } = [];

    // hk4e GadgetClientComp — target entities for client interaction
    public List<uint> TargetEntityIds { get; } = [];

    public override Position Position => _position;
    public override Position Rotation => _rotation;
    public override uint EntityTypeId => (uint)EntityIdTypeEnum.Gadget;

    protected EntityBaseGadget(Scene scene, Position? pos = null, Position? rot = null,
        int campId = 0, int campType = 0) : base(scene)
    {
        _position = pos?.Clone() ?? new Position();
        _rotation = rot?.Clone() ?? new Position();
        CampId = campId;
        CampType = campType;
    }

    // hk4e Gadget::setState — updates state and records timestamp
    public void SetState(uint state)
    {
        GadgetState = state;
        StateBeginTime = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    // hk4e Gadget::isEnableInteract
    public void SetEnableInteract(bool enable) => IsEnableInteract = enable;

    // hk4e Gadget::setBornType
    public void SetBornType(GadgetBornType type) => BornType = type;

    // hk4e Gadget::setPropOwner
    public void SetPropOwner(PlayerInstance? owner) => PropOwner = owner;

    // hk4e Gadget::setFoundationWtr
    public void SetFoundationGadget(EntityBaseGadget? foundation) => FoundationGadget = foundation;

    public bool HasInteractUid(uint uid) => InteractUids.Contains(uid);

    public void AddInteractUid(uint uid)
    {
        if (!InteractUids.Contains(uid))
            InteractUids.Add(uid);
    }
}

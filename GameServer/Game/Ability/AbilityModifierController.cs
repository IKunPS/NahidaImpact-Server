using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;

namespace NahidaImpact.GameServer.Game.Ability;

public class AbilityModifierController
{
    public Ability Ability { get; }
    public AbilityData AbilityData { get; }
    public AbilityModifier ModifierData { get; }

    public uint ModifierId { get; set; }
    public bool IsMuteRemote { get; set; }
    public uint AttachedModifierId { get; set; }
    public uint AttachedModifierOwnerEntityId { get; set; }
    public uint ApplyEntityId { get; set; }
    public bool IsAttachedParentAbility { get; set; }
    public bool IsServerBuffModifier { get; set; }
    public uint ServerBuffUid { get; set; }
    public int ModifierNameHash { get; set; }
    public int AttachNameHash { get; set; }

    public Dictionary<string, int> PileIdxMap { get; } = [];

    public Ability? ParentAbility { get; set; }
    public BaseEntity? OwnerEntity { get; set; }

    public int ParentOwnerIdx { get; set; }
    public long StartTimeMs { get; set; }
    public int ExistDurationMs { get; set; }

    // hk4e ElementDurabilityInfo — element gauge tracking
    public float ElementReduceRatio { get; set; }
    public float ElementRemainingDurability { get; set; }
    public long ElementLastTickTimeMs { get; set; }

    // hk4e PileValue integration — aggregated modifier properties via stacking
    public PileValue? DurabilityPile { get; set; }
    public PileBoolValue? LockHpPile { get; set; }
    public PileBoolValue? NoHealPile { get; set; }

    public bool IsExpired(long nowMs)
        => ExistDurationMs > 0 && (nowMs - StartTimeMs) >= ExistDurationMs;

    public bool IsDurabilityExpired => ElementRemainingDurability <= 0f;

    public AbilityModifierController(Ability ability, AbilityData abilityData, AbilityModifier modifierData)
    {
        Ability = ability;
        AbilityData = abilityData;
        ModifierData = modifierData;
        ParentAbility = ability;
    }

    public override string ToString()
        => $"Modifier: {ModifierData.ModifierName} Id:{ModifierId} Ability:{AbilityData.AbilityName}";
}

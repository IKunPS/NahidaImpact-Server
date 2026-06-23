using NahidaImpact.Data;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.Proto;
using NahidaImpact.Util;

namespace NahidaImpact.GameServer.Game.Ability;

public class Ability
{
    public AbilityData Data { get; }
    public BaseEntity Owner { get; }
    public PlayerInstance PlayerOwner { get; }
    public AbilityManager Manager { get; }

    public Dictionary<string, AbilityModifierController> Modifiers { get; } = [];
    public Dictionary<string, float> AbilitySpecials { get; } = [];

    public Dictionary<int, float> OverrideMap { get; } = [];
    public Dictionary<int, float> ServerOverrideMap { get; } = [];

    public int Hash { get; }
    public HashSet<int> AvatarSkillStartIds { get; }

    public uint AbilityId { get; set; }
    public float ArgumentSpecialValue { get; set; }
    public bool ArgumentReceived { get; set; }
    public float ElemStrength { get; set; }
    public bool IsLevelElementAbility { get; set; }
    public BaseEntity? Caster { get; set; }

    public Ability(AbilityData data, BaseEntity owner, PlayerInstance playerOwner)
    {
        Data = data;
        Owner = owner;
        PlayerOwner = playerOwner;
        Manager = playerOwner.AbilityManager;
        Caster = owner;

        foreach (var entry in Data.AbilitySpecials)
            AbilitySpecials[entry.Key] = entry.Value;

        Hash = (int)Utils.AbilityHash(data.AbilityName);

        Data.Initialize();

        AvatarSkillStartIds = [];
        if (Data.OnAbilityStart != null)
        {
            foreach (var action in Data.OnAbilityStart)
                if (action.Type == "AvatarSkillStart")
                    AvatarSkillStartIds.Add(action.SkillID);
        }

        foreach (var modifier in Data.Modifiers.Values)
        {
            if (modifier.OnAdded != null)
                foreach (var action in modifier.OnAdded)
                    if (action.Type == "AvatarSkillStart")
                        AvatarSkillStartIds.Add(action.SkillID);
        }

        if (Data.OnAdded != null)
            ProcessOnAddedAbilityModifiers();
    }

    private void ProcessOnAddedAbilityModifiers()
    {
        foreach (var action in Data.OnAdded)
        {
            if (action.Type != "ApplyModifier" || string.IsNullOrEmpty(action.ModifierName))
                continue;
            if (!Data.Modifiers.TryGetValue(action.ModifierName, out var modifierData))
                continue;

            Owner.OnAddAbilityModifier(modifierData);
        }
    }

    // Resolves an AbilityString proto to its ability name (both Str and Hash forms)
    public static string? GetAbilityName(AbilityString? abString)
    {
        if (abString == null) return null;
        if (abString.HasStr) return abString.Str;
        if (abString.HasHash) return GameData.GetAbilityNameByHash(abString.Hash);
        return null;
    }

    public override string ToString()
        => $"Ability: {Data.AbilityName} Owner: {Owner}";
}
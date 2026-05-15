using NahidaImpact.Data;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.Proto;
using NahidaImpact.Util;
using System.Collections.Generic;
using System.Linq;

namespace NahidaImpact.GameServer.Game.Ability;

public class Ability
{
    public AbilityData Data { get; }
    public BaseEntity Owner { get; }
    public PlayerInstance PlayerOwner { get; }
    public AbilityManager Manager { get; }

    public Dictionary<string, AbilityModifierController> Modifiers { get; } = new();
    public Dictionary<string, float> AbilitySpecials { get; } = new();

    public int Hash { get; }
    public HashSet<int> AvatarSkillStartIds { get; }

    public Ability(AbilityData data, BaseEntity owner, PlayerInstance playerOwner)
    {
        Data = data;
        Owner = owner;
        PlayerOwner = playerOwner;
        Manager = playerOwner.AbilityManager;

        if (Data.AbilitySpecials != null)
        {
            foreach (var entry in Data.AbilitySpecials)
            {
                AbilitySpecials[entry.Key] = entry.Value;
            }
        }

        Hash = (int)Utils.AbilityHash(data.AbilityName);

        Data.Initialize();

        // Collect AvatarSkillStart IDs from onAbilityStart and all modifier onAdded actions
        AvatarSkillStartIds = new HashSet<int>();
        if (Data.OnAbilityStart != null)
        {
            foreach (var action in Data.OnAbilityStart)
            {
                if (action.Type == "AvatarSkillStart")
                    AvatarSkillStartIds.Add(action.SkillID);
            }
        }

        foreach (var modifier in Data.Modifiers.Values)
        {
            if (modifier.OnAdded != null)
            {
                foreach (var action in modifier.OnAdded)
                {
                    if (action.Type == "AvatarSkillStart")
                        AvatarSkillStartIds.Add(action.SkillID);
                }
            }
        }

        if (Data.OnAdded != null)
        {
            ProcessOnAddedAbilityModifiers();
        }
    }

    public void ProcessOnAddedAbilityModifiers()
    {
        foreach (var modifierAction in Data.OnAdded)
        {
            if (string.IsNullOrEmpty(modifierAction.Type)) continue;

            if (modifierAction.Type == "ApplyModifier")
            {
                if (string.IsNullOrEmpty(modifierAction.ModifierName)) continue;
                if (!Data.Modifiers.ContainsKey(modifierAction.ModifierName)) continue;

                var modifierData = Data.Modifiers[modifierAction.ModifierName];
                Owner.OnAddAbilityModifier(modifierData);
            }
        }
    }

    /// <summary>
    /// Resolves an AbilityString proto to its ability name.
    /// Handles both Str and Hash forms. Mirrors Java Ability.getAbilityName().
    /// </summary>
    public static string? GetAbilityName(AbilityString? abString)
    {
        if (abString == null) return null;
        if (abString.HasStr) return abString.Str;
        if (abString.HasHash) return GameData.GetAbilityNameByHash(abString.Hash);
        return null;
    }

    public override string ToString()
    {
        return $"Ability Name: {Data.AbilityName}; Entity Owner: {Owner}; Player Owner: {PlayerOwner}";
    }
}
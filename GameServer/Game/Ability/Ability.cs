using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.GameServer.Game.Player;
using System.Collections.Generic;

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
        
        Hash = data.AbilityName.GetHashCode();
        
        Data.Initialize();
        
        AvatarSkillStartIds = new HashSet<int>();
        if (Data.OnAbilityStart != null)
        {
            foreach (var action in Data.OnAbilityStart)
            {
                if (action.Type == "AvatarSkillStart")
                {
                    AvatarSkillStartIds.Add(action.SkillID);
                }
            }
        }
        
        foreach (var modifier in Data.Modifiers.Values)
        {
            if (modifier.OnAdded != null)
            {
                foreach (var action in modifier.OnAdded)
                {
                    if (action.Type == "AvatarSkillStart")
                    {
                        AvatarSkillStartIds.Add(action.SkillID);
                    }
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
                // TODO: Implement OnAddAbilityModifier
                // Owner.OnAddAbilityModifier(modifierData);
            }
        }
    }
    
    public static string GetAbilityName(AbilityString abString)
    {
        // TODO: Implement AbilityString resolution
        return abString?.Str ?? string.Empty;
    }
    
    public override string ToString()
    {
        return $"Ability Name: {Data.AbilityName}; Entity Owner: {Owner}; Player Owner: {PlayerOwner}";
    }
}
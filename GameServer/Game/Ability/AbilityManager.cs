using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.Proto;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using NahidaImpact.Data;

namespace NahidaImpact.GameServer.Game.Ability;

public class AbilityManager
{
    private static readonly Dictionary<string, AbilityActionHandler> _actionHandlers = new();
    private static readonly Dictionary<AbilityMixinData.MixinType, AbilityMixinHandler> _mixinHandlers = new();
    
    private readonly PlayerInstance _player;
    
    public bool AbilityInvulnerable { get; set; }
    private int _burstCasterId;
    private int _burstSkillId;
    
    public AbilityManager(PlayerInstance player)
    {
        _player = player;
        RemovePendingEnergyClear();
    }
    
    public void RemovePendingEnergyClear()
    {
        _burstCasterId = 0;
        _burstSkillId = 0;

        RegisterHandlers();
    }
    
    public static void RegisterHandlers()
    {
        var assembly = Assembly.GetExecutingAssembly();
        foreach (var type in assembly.GetTypes())
        {
            if (typeof(AbilityActionHandler).IsAssignableFrom(type) && !type.IsAbstract)
            {
                var attr = type.GetCustomAttribute<AbilityActionAttribute>();
                if (attr != null)
                {
                    var handler = (AbilityActionHandler)Activator.CreateInstance(type);
                    _actionHandlers[attr.ActionType] = handler;
                }
            }
            
            if (typeof(AbilityMixinHandler).IsAssignableFrom(type) && !type.IsAbstract)
            {
                var attr = type.GetCustomAttribute<AbilityMixinAttribute>();
                if (attr != null)
                {
                    var handler = (AbilityMixinHandler)Activator.CreateInstance(type);
                    _mixinHandlers[attr.MixinType] = handler;
                }
            }
        }
    }
    
    public async Task ExecuteAction(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        if (!_actionHandlers.TryGetValue(action.Type, out var handler) || ability == null)
        {
            // Log missing handler
            return;
        }
        
        await handler.Execute(ability, action, abilityData, target);
    }
    
    public async Task ExecuteMixin(Ability ability, AbilityMixinData mixinData, ByteString abilityData)
    {
        if (!_mixinHandlers.TryGetValue(mixinData.Type, out var handler) || ability == null)
        {
            // Log missing handler
            return;
        }
        
        await handler.Execute(ability, mixinData, abilityData, ability.Owner);
    }
    
    public void OnAbilityInvoke(AbilityInvokeEntry invoke)
    {
        var entity = _player.Scene?.GetEntityById((int)invoke.EntityId);
        if (entity == null) return;
        
        switch (invoke.ArgumentType)
        {
            case AbilityInvokeArgument.AbilityMetaOverrideParam:
                HandleOverrideParam(invoke, entity);
                break;
            case AbilityInvokeArgument.AbilityMetaReinitOverridemap:
                HandleReinitOverrideMap(invoke, entity);
                break;
            case AbilityInvokeArgument.AbilityMetaModifierChange:
                HandleModifierChange(invoke, entity);
                break;
            case AbilityInvokeArgument.AbilityMetaAddNewAbility:
                HandleAddNewAbility(invoke, entity);
                break;
            default:
                // Log unsupported argument type
                break;
        }
    }
    
    public void OnSkillStart(PlayerInstance player, int skillId, int casterId)
    {
        if (player.Uid != _player.Uid) return;
        
        // TODO: Implement skill start logic
    }
    
    public void OnSkillEnd(PlayerInstance player)
    {
        if (player.Uid != _player.Uid) return;
        
        if (!AbilityInvulnerable) return;
        
        AbilityInvulnerable = false;
    }
    
    public void AddAbilityToEntity(BaseEntity entity, string abilityName)
    {
        if (GameData.AbilityData.TryGetValue(abilityName, out var abilityData))
        {
            AddAbilityToEntity(entity, abilityData);
        }
    }
    
    public void AddAbilityToEntity(BaseEntity entity, AbilityData abilityData)
    {
        var ability = new Ability(abilityData, entity, _player);
        entity.InstancedAbilities.Add(ability);
    }
    
    private void HandleOverrideParam(AbilityInvokeEntry invoke, BaseEntity entity)
    {
        // TODO: Implement
    }
    
    private void HandleReinitOverrideMap(AbilityInvokeEntry invoke, BaseEntity entity)
    {
        // TODO: Implement
    }
    
    private void HandleModifierChange(AbilityInvokeEntry invoke, BaseEntity entity)
    {
        // TODO: Implement
    }
    
    private void HandleAddNewAbility(AbilityInvokeEntry invoke, BaseEntity entity)
    {
        // TODO: Implement
    }
}
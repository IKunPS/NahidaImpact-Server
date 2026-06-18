using NahidaImpact.Data;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.GameServer.Game.Player;
using Google.Protobuf;
using System.Reflection;
using NahidaImpact.GameServer.Server.Packet.Send.Ability;


namespace NahidaImpact.GameServer.Game.Ability;

public class AbilityManager
{
    private static readonly Dictionary<string, AbilityActionHandler> _actionHandlers = new();
    private static readonly Dictionary<AbilityMixinData.MixinType, AbilityMixinHandler> _mixinHandlers = new();
    private static readonly Util.Logger _logger = new("AbilityManager");
    private readonly PlayerInstance _player;

    public bool AbilityInvulnerable { get; set; }
    private int _burstCasterId;
    private int _burstSkillId;
    
    public InvokeHandler<AbilityInvokeEntry> AbilityInvokeHandler { get; }
    public InvokeHandler<AbilityInvokeEntry> ClientAbilityInitFinishHandler { get; }

    public AbilityManager(PlayerInstance player)
    {
        _player = player;
        AbilityInvokeHandler = new InvokeHandler<AbilityInvokeEntry>(PacketAbilityInvocationsNotify);
        ClientAbilityInitFinishHandler = new InvokeHandler<AbilityInvokeEntry>(PacketClientAbilityInitFinishNotify);
        RemovePendingEnergyClear();

        lock (_actionHandlers)
        {
            if (_actionHandlers.Count == 0)
                RegisterHandlers();
        }
    }

    public PlayerInstance GetPlayer() => _player;

    public void RemovePendingEnergyClear()
    {
        _burstCasterId = 0;
        _burstSkillId = 0;
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
                    var handler = (AbilityActionHandler)Activator.CreateInstance(type)!;
                    _actionHandlers[attr.ActionType] = handler;
                }
            }

            if (typeof(AbilityMixinHandler).IsAssignableFrom(type) && !type.IsAbstract)
            {
                var attr = type.GetCustomAttribute<AbilityMixinAttribute>();
                if (attr != null)
                {
                    var handler = (AbilityMixinHandler)Activator.CreateInstance(type)!;
                    _mixinHandlers[attr.MixinType] = handler;
                }
            }
        }
    }

    public void ExecuteAction(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        if (!_actionHandlers.TryGetValue(action.Type, out var handler) || ability == null)
            return;

        if (handler.AbilityManager == null)
            handler.AbilityManager = this;

        _ = Task.Run(async () =>
        {
            if (!await handler.Execute(ability, action, abilityData, target))
            {
                _logger.Debug($"Ability execute action failed for {action.Type} at {ability}");
            }
        });
    }

    public void ExecuteMixin(Ability ability, AbilityMixinData mixinData, ByteString abilityData)
    {
        if (!_mixinHandlers.TryGetValue(mixinData.Type, out var handler) || ability == null)
            return;

        var target = ability.Owner;
        _ = Task.Run(async () =>
        {
            if (!await handler.Execute(ability, mixinData, abilityData, target))
            {
                _logger.Error($"Ability execute mixin failed for {mixinData.Type} at {ability}");
            }
        });
    }

    public void OnAbilityInvoke(AbilityInvokeEntry invoke)
    {
        var head = invoke.Head;
        var entity = _player.Scene?.GetEntityById((int)invoke.EntityId);
        if (entity == null) return;
        
        if (head != null && head.LocalId != 0)
        {
            HandleServerInvoke(invoke, entity);
        }

        switch (invoke.ArgumentType)
        {
            case AbilityInvokeArgument.MetaOverrideParam:
                HandleOverrideParam(invoke, entity);
                break;
            case AbilityInvokeArgument.MetaReinitOverridemap:
                HandleReinitOverrideMap(invoke, entity);
                break;
            case AbilityInvokeArgument.MetaModifierChange:
                HandleModifierChange(invoke, entity);
                break;
            case AbilityInvokeArgument.MetaGlobalFloatValue:
                HandleGlobalFloatValue(invoke, entity);
                break;
            case AbilityInvokeArgument.MetaClearGlobalFloatValue:
                HandleClearGlobalFloatValue(invoke, entity);
                break;
            case AbilityInvokeArgument.MetaAddNewAbility:
                HandleAddNewAbility(invoke, entity);
                break;
            case AbilityInvokeArgument.MetaSetKilledSetate:
                HandleKillState(invoke, entity);
                break;
            case AbilityInvokeArgument.MetaAddSpecialEnergyValue:
                HandleAddSpecialEnergy(invoke, entity);
                break;
            case AbilityInvokeArgument.MixinChangePhlogiston:
                break;
        }
    }

    private static void SetAbilityOverrideValue(Ability ability, AbilityScalarValueEntry valueChange)
    {
        if (valueChange?.Key == null) return;

        string? key;
        if (valueChange.Key.HasStr)
            key = valueChange.Key.Str;
        else if (valueChange.Key.HasHash)
            key = GameData.GetAbilityNameByHash(valueChange.Key.Hash);
        else
            return;

        if (key == null) return;

        ability.AbilitySpecials[key] = valueChange.FloatValue;
    }

    private static void HandleOverrideParam(AbilityInvokeEntry invoke, BaseEntity entity)
    {
        var head = invoke.Head;
        if (head == null) return;

        var instancedAbilityIndex = (int)head.InstancedAbilityId - 1;
        if (instancedAbilityIndex >= entity.InstancedAbilities.Count) return;

        var ability = entity.InstancedAbilities[instancedAbilityIndex];

        try
        {
            var valueChange = AbilityScalarValueEntry.Parser.ParseFrom(invoke.AbilityData);
            SetAbilityOverrideValue(ability, valueChange);
        }
        catch (Exception) { }
    }

    private static void HandleReinitOverrideMap(AbilityInvokeEntry invoke, BaseEntity entity)
    {
        var head = invoke.Head;
        if (head == null) return;

        var instancedAbilityIndex = (int)head.InstancedAbilityId - 1;
        if (instancedAbilityIndex >= entity.InstancedAbilities.Count) return;

        var ability = entity.InstancedAbilities[instancedAbilityIndex];

        try
        {
            var valueChanges = AbilityMetaReInitOverrideMap.Parser.ParseFrom(invoke.AbilityData);
            foreach (var varChange in valueChanges.OverrideMap)
            {
                SetAbilityOverrideValue(ability, varChange);
            }
        }
        catch (Exception) { }
    }

    private void OnPossibleElementalBurst(Ability ability, AbilityModifier modifier, int entityId)
    {
        if (_burstCasterId == 0) return;

        bool skillInvincibility = modifier.State == State.Invincible;
        if (modifier.OnAdded != null)
        {
            skillInvincibility |= modifier.OnAdded.Any(action =>
                action.Type == "AttachAbilityStateResistance" && action.ResistanceListID == 11002);
        }

        if (_burstCasterId == entityId
            && (ability.AvatarSkillStartIds.Contains(_burstSkillId) || skillInvincibility))
        {
            _logger.Debug($"Caster ID {entityId} burst successful, clearing energy and setting invulnerability");
            AbilityInvulnerable = true;
            // TODO: _player.EnergyManager?.HandleEvtDoSkillSuccNotify(_player.Session, _burstSkillId, _burstCasterId);
            RemovePendingEnergyClear();
        }
    }

    private void HandleModifierChange(AbilityInvokeEntry invoke, BaseEntity entity)
    {
        AbilityMetaModifierChange modChange;
        try
        {
            modChange = AbilityMetaModifierChange.Parser.ParseFrom(invoke.AbilityData);
        }
        catch (Exception) { return; }

        var head = invoke.Head;
        if (head == null) return;

        // _Transfer_Vehicle special case
        var parentName = modChange?.ParentAbilityName;
        if (parentName != null && parentName.Str != null && parentName.Str.Contains("_Transfer_Vehicle"))
        {
            // TODO: Implement vehicle transfer when EntityVehicle and Scene entity management are ready
            return;
        }

        if (head.InstancedAbilityId == 0 || head.InstancedModifierId > 2000)
            return;

        if (modChange.Action == ModifierAction.Added)
        {
            AbilityData? instancedAbilityData = null;
            Ability? instancedAbility = null;

            // Try target entity first
            if (head.TargetId != 0)
            {
                var targetEntity = _player.Scene?.GetEntityById((int)head.TargetId);
                if (targetEntity != null)
                {
                    if (head.InstancedAbilityId - 1 < targetEntity.InstancedAbilities.Count)
                    {
                        instancedAbility = targetEntity.InstancedAbilities[(int)(head.InstancedAbilityId - 1)];
                        if (instancedAbility != null) instancedAbilityData = instancedAbility.Data;
                    }
                }
            }

            // Fallback to source entity
            if (instancedAbilityData == null)
            {
                if (head.InstancedAbilityId - 1 < entity.InstancedAbilities.Count)
                {
                    instancedAbility = entity.InstancedAbilities[(int)(head.InstancedAbilityId - 1)];
                    if (instancedAbility != null) instancedAbilityData = instancedAbility.Data;
                }
            }

            // Fallback to parent ability name lookup
            if (instancedAbilityData == null)
            {
                var pn = modChange?.ParentAbilityName;
                if (pn != null && !string.IsNullOrEmpty(pn.Str))
                    instancedAbilityData = GameData.GetAbilityData(pn.Str);
            }

            if (instancedAbilityData == null) return;

            var modifierArray = instancedAbilityData.Modifiers.Values.ToArray();
            if (modChange.ModifierLocalId >= modifierArray.Length) return;

            var modifierData = modifierArray[modChange.ModifierLocalId];

            if (instancedAbility != null)
            {
                OnPossibleElementalBurst(instancedAbility, modifierData, (int)invoke.EntityId);
            }

            var modifier = new AbilityModifierController(instancedAbility!, instancedAbilityData, modifierData);
            entity.InstancedModifiers[(int)head.InstancedModifierId] = modifier;

            _logger.Debug($"Added entity {(int)invoke.EntityId} modifier id {head.InstancedModifierId} " +
                $"with ability {instancedAbilityData.AbilityName}");
        }
        else if (modChange.Action == ModifierAction.Removed)
        {
            _logger.Debug($"Removed on entity {(int)invoke.EntityId} modifier id {head.InstancedModifierId}");
            entity.InstancedModifiers.Remove((int)head.InstancedModifierId);
        }
    }

    private static void HandleGlobalFloatValue(AbilityInvokeEntry invoke, BaseEntity entity)
    {
        try
        {
            var entry = AbilityScalarValueEntry.Parser.ParseFrom(invoke.AbilityData);
            if (entry?.Key == null) return;

            string? key = null;
            if (entry.Key.HasStr) key = entry.Key.Str;
            else if (entry.Key.HasHash) key = GameData.GetAbilityNameByHash(entry.Key.Hash);

            if (key == null) return;

            if (!float.IsNaN(entry.FloatValue))
                entity.GlobalAbilityValues[key] = entry.FloatValue;

            entity.OnAbilityValueUpdate();
        }
        catch (Exception) { }
    }

    private static void HandleClearGlobalFloatValue(AbilityInvokeEntry invoke, BaseEntity entity)
    {
        try
        {
            var entry = AbilityScalarValueEntry.Parser.ParseFrom(invoke.AbilityData);
            if (entry?.Key == null) return;

            string? key = null;
            if (entry.Key.HasStr) key = entry.Key.Str;
            else if (entry.Key.HasHash) key = GameData.GetAbilityNameByHash(entry.Key.Hash);

            if (key == null) return;

            entity.GlobalAbilityValues.Remove(key);
            entity.OnAbilityValueUpdate();
        }
        catch (Exception) { }
    }

    // ===== Add New Ability =====

    private void HandleAddNewAbility(AbilityInvokeEntry invoke, BaseEntity entity)
    {
        try
        {
            var addAbility = AbilityMetaAddAbility.Parser.ParseFrom(invoke.AbilityData);
            var abilityName = Ability.GetAbilityName(addAbility?.Ability?.AbilityName);
            var abilityData = GameData.GetAbilityData(abilityName);
            if (abilityData == null)
            {
                _logger.Debug($"Ability not found in AddNewAbility: {abilityName}");
                return;
            }

            entity.InstancedAbilities.Add(new Ability(abilityData, entity, _player));
            _logger.Debug($"Ability added to entity {entity.Id} at index {entity.InstancedAbilities.Count}.");
        }
        catch (Exception) { }
    }

    private static void HandleKillState(AbilityInvokeEntry invoke, BaseEntity entity)
    {
        // TODO: Parse AbilityMetaSetKilledState when proto is available
    }

    private void HandleAddSpecialEnergy(AbilityInvokeEntry invoke, BaseEntity entity)
    {
        // TODO: Parse AbilityMetaSpecialEnergy when proto is available
        var head = invoke.Head;
        if (head == null) return;

        var target = head.TargetId != 0 ? _player.Scene?.GetEntityById((int)head.TargetId) : null;
        target ??= entity;
        if (target == null) return;

        // Placeholder: need AbilityMetaSpecialEnergy proto
    }

    public void HandleServerInvoke(AbilityInvokeEntry invoke, BaseEntity entity)
    {
        var head = invoke.Head;
        if (head == null) return;

        var target = head.TargetId != 0 ? _player.Scene?.GetEntityById((int)head.TargetId) : null;
        target ??= entity;

        Ability? ability = null;

        // Find ability from instanced modifier ID
        if (head.InstancedModifierId != 0
            && entity.InstancedModifiers.TryGetValue((int)head.InstancedModifierId, out var modCtrl))
        {
            ability = modCtrl.Ability;
        }

        // Find ability from instanced ability ID
        if (ability == null
            && head.InstancedAbilityId != 0
            && head.InstancedAbilityId - 1 < entity.InstancedAbilities.Count)
        {
            ability = entity.InstancedAbilities[(int)(head.InstancedAbilityId - 1)];
        }

        if (ability == null)
        {
            _logger.Debug($"Ability not found: ability {head.InstancedAbilityId} modifier {head.InstancedModifierId}");
            return;
        }

        // Build DetailAbilityInfo placeholder (proto not yet available)
        // entity.DetailAbilityInfo = ...;

        // Dispatch by local ID to action or mixin
        var action = ability.Data.LocalIdToAction.GetValueOrDefault((int)head.LocalId);
        if (action != null)
        {
            ExecuteAction(ability, action, invoke.AbilityData, target);
            return;
        }

        var mixin = ability.Data.LocalIdToMixin.GetValueOrDefault((int)head.LocalId);
        if (mixin != null)
        {
            ExecuteMixin(ability, mixin, invoke.AbilityData);
            return;
        }

        // Phlogiston revive gadget handling
        if (ability.Data.AbilityName == "SceneObj_Area_Nt_Property_Prop_PhlogistonRevive")
        {
            // TODO: Handle gadget state update when EntityGadget is available
        }

        _logger.Debug($"Action or mixin not found: local_id {head.LocalId} ability {ability.Data.AbilityName}");
    }

    public void OnSkillStart(PlayerInstance player, int skillId, int casterId)
    {
        if (player.Uid != _player.Uid) return;

        var currentAvatar = player.TeamManager?.GetCurrentAvatarEntity();
        if (currentAvatar == null || currentAvatar.Id != casterId)
            return;

        // TODO: Look up AvatarSkillData when available
        // var skillData = GameData.GetAvatarSkillData(skillId);
        // if (skillData == null) return;
        // if (skillData.CostElemVal <= 0) return;

        _burstSkillId = skillId;
        _burstCasterId = casterId;
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

    private void PacketAbilityInvocationsNotify(List<AbilityInvokeEntry> entries)
    {
        var packet = new PacketAbilityInvocationsNotify(entries);
        _ = _player.SendPacket(packet);
    }

    private void PacketClientAbilityInitFinishNotify(List<AbilityInvokeEntry> entries)
    {
        var packet = new PacketClientAbilityInitFinishNotify(entries);
        _ = _player.SendPacket(packet);
    }
}

public class InvokeHandler<T>
{
    private readonly Action<List<T>> _sendAction;
    private readonly List<T> _entryList = new();
    private readonly object _lock = new();

    public InvokeHandler(Action<List<T>> sendAction)
    {
        _sendAction = sendAction;
    }

    public void AddEntry(T entry)
    {
        lock (_lock) { _entryList.Add(entry); }
    }

    public void Clear()
    {
        lock (_lock) { _entryList.Clear(); }
    }

    public void Send()
    {
        lock (_lock)
        {
            if (_entryList.Count > 0)
            {
                _sendAction(new List<T>(_entryList));
                _entryList.Clear();
            }
        }
    }

    public int Count
    {
        get { lock (_lock) { return _entryList.Count; } }
    }
}

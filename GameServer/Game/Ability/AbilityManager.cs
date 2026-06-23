using System.Reflection;
using Google.Protobuf;
using NahidaImpact.Data;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.GameServer.Server.Packet.Send.Ability;
using NahidaImpact.Proto;
using NahidaImpact.Util;

namespace NahidaImpact.GameServer.Game.Ability;

public class AbilityManager
{
    private static readonly Dictionary<string, AbilityActionHandler> ActionHandlers = [];
    private static readonly Dictionary<AbilityMixinData.MixinType, AbilityMixinHandler> MixinHandlers = [];
    private static readonly Util.Logger Logger = new("AbilityManager");

    private readonly PlayerInstance _player;
    private int _burstCasterId;
    private int _burstSkillId;

    public bool AbilityInvulnerable { get; set; }
    public InvokeHandler<AbilityInvokeEntry> AbilityInvokeHandler { get; }
    public InvokeHandler<AbilityInvokeEntry> ClientAbilityInitFinishHandler { get; }

    public PlayerInstance Player => _player;

    public AbilityManager(PlayerInstance player)
    {
        _player = player;
        AbilityInvokeHandler = new InvokeHandler<AbilityInvokeEntry>(PacketAbilityInvocationsNotify);
        ClientAbilityInitFinishHandler = new InvokeHandler<AbilityInvokeEntry>(PacketClientAbilityInitFinishNotify);
        RemovePendingEnergyClear();

        lock (ActionHandlers)
        {
            if (ActionHandlers.Count == 0)
                RegisterHandlers();
        }
    }

    #region Handler Registration

    private static void RegisterHandlers()
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
                    ActionHandlers[attr.ActionType] = handler;
                }
            }

            if (typeof(AbilityMixinHandler).IsAssignableFrom(type) && !type.IsAbstract)
            {
                var attr = type.GetCustomAttribute<AbilityMixinAttribute>();
                if (attr != null)
                {
                    var handler = (AbilityMixinHandler)Activator.CreateInstance(type)!;
                    MixinHandlers[attr.MixinType] = handler;
                }
            }
        }
    }

    #endregion

    #region Action / Mixin Dispatch

    public void ExecuteAction(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        if (!ActionHandlers.TryGetValue(action.Type, out var handler) || ability == null)
            return;

        if (handler.AbilityManager == null)
            handler.AbilityManager = this;

        _ = Task.Run(async () =>
        {
            if (!await handler.Execute(ability, action, abilityData, target))
                Logger.Debug($"Action failed: {action.Type} ability:{ability.Data.AbilityName}");
        });
    }

    public void ExecuteMixin(Ability ability, AbilityMixinData mixinData, ByteString abilityData)
    {
        if (!MixinHandlers.TryGetValue(mixinData.Type, out var handler) || ability == null)
            return;

        var target = ability.Owner;
        _ = Task.Run(async () =>
        {
            if (!await handler.Execute(ability, mixinData, abilityData, target))
                Logger.Error($"Mixin failed: {mixinData.Type} ability:{ability.Data.AbilityName}");
        });
    }

    #endregion

    #region Client Invoke Entry Point

    public void OnAbilityInvoke(AbilityInvokeEntry invoke)
    {
        var head = invoke.Head;
        var entity = _player.Scene?.GetEntityById((int)invoke.EntityId);
        if (entity == null) return;

        if (head != null && head.LocalId != 0)
            HandleServerInvoke(invoke, entity);

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
            case AbilityInvokeArgument.MetaModifierDurabilityChange:
                HandleModifierDurabilityChange(invoke, entity);
                break;
            case AbilityInvokeArgument.MixinChangePhlogiston:
                break;
        }
    }

    public void HandleServerInvoke(AbilityInvokeEntry invoke, BaseEntity entity)
    {
        var head = invoke.Head;
        if (head == null) return;

        var target = head.TargetId != 0 ? _player.Scene?.GetEntityById((int)head.TargetId) : null;
        target ??= entity;

        Ability? ability = null;

        if (head.InstancedModifierId != 0
            && entity.InstancedModifiers.TryGetValue((int)head.InstancedModifierId, out var modCtrl))
        {
            ability = modCtrl.Ability;
        }

        if (ability == null
            && head.InstancedAbilityId != 0
            && head.InstancedAbilityId - 1 < entity.InstancedAbilities.Count)
        {
            ability = entity.InstancedAbilities[(int)(head.InstancedAbilityId - 1)];
        }

        if (ability == null) return;

        ability.Caster = entity;

        if (ability.Data.LocalIdToAction.TryGetValue((int)head.LocalId, out var action))
        {
            ExecuteAction(ability, action, invoke.AbilityData, target);
            return;
        }

        if (ability.Data.LocalIdToMixin.TryGetValue((int)head.LocalId, out var mixin))
        {
            ExecuteMixin(ability, mixin, invoke.AbilityData);
        }
    }

    #endregion

    #region Override Param / Override Map

    private static void SetAbilityOverrideValue(Ability ability, AbilityScalarValueEntry entry)
    {
        if (entry?.Key == null) return;

        string? key;
        if (entry.Key.HasStr)
            key = entry.Key.Str;
        else if (entry.Key.HasHash)
            key = GameData.GetAbilityNameByHash(entry.Key.Hash);
        else
            return;

        if (key == null) return;

        ability.AbilitySpecials[key] = entry.FloatValue;
        ability.OverrideMap[(int)Utils.AbilityHash(key)] = entry.FloatValue;
    }

    private static void HandleOverrideParam(AbilityInvokeEntry invoke, BaseEntity entity)
    {
        var head = invoke.Head;
        if (head == null) return;

        var idx = (int)head.InstancedAbilityId - 1;
        if (idx >= entity.InstancedAbilities.Count) return;

        var ability = entity.InstancedAbilities[idx];

        try
        {
            var entry = AbilityScalarValueEntry.Parser.ParseFrom(invoke.AbilityData);
            SetAbilityOverrideValue(ability, entry);
        }
        catch (Exception) { /* proto parse failure is non-critical */ }
    }

    private static void HandleReinitOverrideMap(AbilityInvokeEntry invoke, BaseEntity entity)
    {
        var head = invoke.Head;
        if (head == null) return;

        var idx = (int)head.InstancedAbilityId - 1;
        if (idx >= entity.InstancedAbilities.Count) return;

        var ability = entity.InstancedAbilities[idx];

        try
        {
            var reinit = AbilityMetaReInitOverrideMap.Parser.ParseFrom(invoke.AbilityData);
            foreach (var entry in reinit.OverrideMap)
                SetAbilityOverrideValue(ability, entry);
        }
        catch (Exception) { }
    }

    #endregion

    #region Modifier Change (Add / Remove)

    private void OnPossibleElementalBurst(Ability ability, AbilityModifier modifier, int entityId)
    {
        if (_burstCasterId == 0) return;

        var invincible = modifier.State == State.Invincible;
        if (modifier.OnAdded != null)
        {
            invincible |= modifier.OnAdded.Any(a =>
                a.Type == "AttachAbilityStateResistance" && a.ResistanceListID == 11002);
        }

        if (_burstCasterId == entityId
            && (ability.AvatarSkillStartIds.Contains(_burstSkillId) || invincible))
        {
            AbilityInvulnerable = true;
            RemovePendingEnergyClear();
        }
    }

    private void HandleModifierChange(AbilityInvokeEntry invoke, BaseEntity entity)
    {
        AbilityMetaModifierChange modChange;
        try { modChange = AbilityMetaModifierChange.Parser.ParseFrom(invoke.AbilityData); }
        catch (Exception) { return; }

        var head = invoke.Head;
        if (head == null) return;

        // Vehicle transfer check
        if (modChange?.ParentAbilityName?.Str?.Contains("_Transfer_Vehicle") == true)
            return;

        if (head.InstancedAbilityId == 0 || head.InstancedModifierId > 2000)
            return;

        if (modChange.Action == ModifierAction.Added)
        {
            AbilityData? abilityData = null;
            Ability? ability = null;

            if (head.TargetId != 0)
            {
                var target = _player.Scene?.GetEntityById((int)head.TargetId);
                if (target != null && head.InstancedAbilityId - 1 < target.InstancedAbilities.Count)
                {
                    ability = target.InstancedAbilities[(int)(head.InstancedAbilityId - 1)];
                    abilityData = ability?.Data;
                }
            }

            if (abilityData == null && head.InstancedAbilityId - 1 < entity.InstancedAbilities.Count)
            {
                ability = entity.InstancedAbilities[(int)(head.InstancedAbilityId - 1)];
                abilityData = ability?.Data;
            }

            if (abilityData == null)
            {
                var pn = modChange?.ParentAbilityName;
                if (pn != null && !string.IsNullOrEmpty(pn.Str))
                    abilityData = GameData.GetAbilityData(pn.Str);
            }

            if (abilityData == null) return;

            var modifiers = abilityData.Modifiers.Values.ToArray();
            if (modChange.ModifierLocalId >= modifiers.Length) return;

            var modifierData = modifiers[modChange.ModifierLocalId];

            if (ability != null)
                OnPossibleElementalBurst(ability, modifierData, (int)invoke.EntityId);

            var attached = modChange.AttachedInstancedModifier;
            var modifier = new AbilityModifierController(ability!, abilityData, modifierData)
            {
                ModifierId = head.InstancedModifierId,
                ApplyEntityId = invoke.EntityId,
                IsAttachedParentAbility = modChange.IsAttachedParentAbility,
                AttachedModifierId = attached?.InstancedModifierId ?? 0,
                AttachedModifierOwnerEntityId = attached?.OwnerEntityId ?? 0,
                StartTimeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                IsMuteRemote = modChange.IsMuteRemote,
                IsServerBuffModifier = modChange.ServerBuffUid != 0,
                ServerBuffUid = modChange.ServerBuffUid,
                ElementRemainingDurability = modChange.IsDurabilityZero ? 0f : modifierData.ElementDurability,
                ElementReduceRatio = 1f
            };

            entity.InstancedModifiers[(int)head.InstancedModifierId] = modifier;
        }
        else if (modChange.Action == ModifierAction.Removed)
        {
            entity.InstancedModifiers.Remove((int)head.InstancedModifierId);
        }
    }

    #endregion

    #region Global Float Value

    private static void HandleGlobalFloatValue(AbilityInvokeEntry invoke, BaseEntity entity)
    {
        try
        {
            var entry = AbilityScalarValueEntry.Parser.ParseFrom(invoke.AbilityData);
            if (entry?.Key == null) return;

            string? key;
            if (entry.Key.HasStr)
                key = entry.Key.Str;
            else if (entry.Key.HasHash)
                key = GameData.GetAbilityNameByHash(entry.Key.Hash);
            else
                return;

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

            string? key;
            if (entry.Key.HasStr)
                key = entry.Key.Str;
            else if (entry.Key.HasHash)
                key = GameData.GetAbilityNameByHash(entry.Key.Hash);
            else
                return;

            if (key == null) return;

            entity.GlobalAbilityValues.Remove(key);
            entity.OnAbilityValueUpdate();
        }
        catch (Exception) { }
    }

    #endregion

    #region Add New Ability

    private void HandleAddNewAbility(AbilityInvokeEntry invoke, BaseEntity entity)
    {
        try
        {
            var addAbility = AbilityMetaAddAbility.Parser.ParseFrom(invoke.AbilityData);
            var abilityName = Ability.GetAbilityName(addAbility?.Ability?.AbilityName);
            var abilityData = GameData.GetAbilityData(abilityName);
            if (abilityData == null) return;

            entity.InstancedAbilities.Add(new Ability(abilityData, entity, _player));
        }
        catch (Exception) { }
    }

    #endregion

    #region Kill State / Special Energy / Durability Change

    private static void HandleKillState(AbilityInvokeEntry invoke, BaseEntity entity)
    {
        try
        {
            var killedState = AbilityMetaSetKilledState.Parser.ParseFrom(invoke.AbilityData);
            if (killedState.Killed)
            {
                entity.IsDead = true;
                entity.OnDeath(0);
            }
        }
        catch (Exception) { }
    }

    private void HandleAddSpecialEnergy(AbilityInvokeEntry invoke, BaseEntity entity)
    {
        var head = invoke.Head;
        if (head == null) return;

        if (_player.Scene == null) return;

        var target = entity;
        if (head.TargetId != 0)
            target = _player.Scene.GetEntityById((int)head.TargetId) ?? entity;

        try
        {
            var specialEnergy = AbilityMetaSpecialEnergy.Parser.ParseFrom(invoke.AbilityData);
            target.AddSpecialEnergy(specialEnergy.Value);
        }
        catch (Exception) { }
    }

    private static void HandleModifierDurabilityChange(AbilityInvokeEntry invoke, BaseEntity entity)
    {
        var head = invoke.Head;
        if (head == null || head.InstancedModifierId == 0) return;

        if (!entity.InstancedModifiers.TryGetValue((int)head.InstancedModifierId, out var modCtrl))
            return;

        try
        {
            var durability = ModifierDurability.Parser.ParseFrom(invoke.AbilityData);
            modCtrl.ElementReduceRatio = durability.ReduceRatio;
            modCtrl.ElementRemainingDurability = durability.RemainingDurability;
            modCtrl.ElementLastTickTimeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
        catch (Exception) { }
    }

    #endregion

    #region Element Skill Burst

    public void RemovePendingEnergyClear()
    {
        _burstCasterId = 0;
        _burstSkillId = 0;
    }

    public void OnSkillStart(PlayerInstance player, int skillId, int casterId)
    {
        if (player.Uid != _player.Uid) return;

        var currentAvatar = player.TeamManager?.GetCurrentAvatarEntity();
        if (currentAvatar == null || currentAvatar.Id != casterId)
            return;

        _burstSkillId = skillId;
        _burstCasterId = casterId;
    }

    public void OnSkillEnd(PlayerInstance player)
    {
        if (player.Uid != _player.Uid) return;
        if (!AbilityInvulnerable) return;
        AbilityInvulnerable = false;
    }

    #endregion

    #region Entity Ability Management

    public void AddAbilityToEntity(BaseEntity entity, string abilityName)
    {
        if (GameData.AbilityData.TryGetValue(abilityName, out var abilityData))
            AddAbilityToEntity(entity, abilityData);
    }

    public void AddAbilityToEntity(BaseEntity entity, AbilityData abilityData)
    {
        var ability = new Ability(abilityData, entity, _player);
        entity.InstancedAbilities.Add(ability);
    }

    #endregion

    #region Network

    private void PacketAbilityInvocationsNotify(List<AbilityInvokeEntry> entries)
    {
        _ = _player.SendPacket(new PacketAbilityInvocationsNotify(entries));
    }

    private void PacketClientAbilityInitFinishNotify(List<AbilityInvokeEntry> entries)
    {
        _ = _player.SendPacket(new PacketClientAbilityInitFinishNotify(entries));
    }

    #endregion
}

public class InvokeHandler<T>
{
    private readonly Action<List<T>> _sendAction;
    private readonly List<T> _entryList = [];
    private readonly object _lock = new();

    public InvokeHandler(Action<List<T>> sendAction) => _sendAction = sendAction;

    public void AddEntry(T entry) { lock (_lock) _entryList.Add(entry); }

    public void Clear() { lock (_lock) _entryList.Clear(); }

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

    public int Count { get { lock (_lock) return _entryList.Count; } }
}

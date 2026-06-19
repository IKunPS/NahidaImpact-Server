using Google.Protobuf;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.GameServer.Game.Worlds;
using NahidaImpact.GameServer.Game.Event.Entity;
using NahidaImpact.GameServer.Server.Packet.Send.Entity;
using NahidaImpact.Prop;
using NahidaImpact.Proto;
using PlayerInstance = NahidaImpact.GameServer.Game.Player.PlayerInstance;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Ability;

[Opcode(CmdIds.CombatInvocationsNotify)]
public class HandlerCombatInvocationsNotify : Handler
{
    // Fall damage tracking fields
    private float _cachedLandingSpeed = 0;
    private long _cachedLandingTimeMillisecond = 0;
    private bool _monitorLandingEvent = false;

    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        if (connection.Player == null)
        {
            connection.Stop();
            return;
        }

        var player = connection.Player;
        var notify = CombatInvocationsNotify.Parser.ParseFrom(data);

        foreach (var entry in notify.InvokeList)
        {
            switch (entry.ArgumentType)
            {
                case CombatTypeArgument.CombatEvtBeingHit:
                    HandleEvtBeingHit(player, entry.CombatData);
                    break;

                case CombatTypeArgument.EntityMove:
                    HandleEntityMove(player, entry);
                    break;

                case CombatTypeArgument.CombatAnimatorParameterChanged:
                    HandleAnimatorParameterChanged(entry);
                    break;
            }

            // Forward the entry to other players
            if (ShouldForward(entry))
            {
                player.CombatInvokeHandler.AddEntry(entry.ForwardType, entry);
            }
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// Handle COMBAT_EVT_BEING_HIT - damage events
    /// </summary>
    private void HandleEvtBeingHit(PlayerInstance player, ByteString combatData)
    {
        var evtBeingHitInfo = EvtBeingHitInfo.Parser.ParseFrom(combatData);
        var attackResult = evtBeingHitInfo.AttackResult;

        // Get attacker and defense entities from scene
        var attacker = player.Scene?.GetEntityById((int)attackResult.AttackerId);
        var defense = player.Scene?.GetEntityById((int)attackResult.DefenseId);

        // Skip if attacker is ability-invulnerable and not the player's current avatar
        if (attacker != null)
        {
            var currentAvatarEntity = player.TeamManager?.GetCurrentAvatarEntity();
            if (currentAvatarEntity != null &&
                attacker != currentAvatarEntity &&
                player.AbilityManager?.AbilityInvulnerable == true)
            {
                return;
            }
        }

        // Apply damage to the defense target entity
        if (defense != null && attackResult.Damage > 0)
        {
            var attackType = (Data.Ability.ElementType)attackResult.ElementType;
            defense.Damage(attackResult.Damage, (int)attackResult.AttackerId, attackType);
        }
    }

    /// <summary>
    /// Handle ENTITY_MOVE - the main movement entry
    /// </summary>
    private void HandleEntityMove(PlayerInstance player, CombatInvokeEntry entry)
    {
        var moveInfo = EntityMoveInfo.Parser.ParseFrom(entry.CombatData);
        var entity = player.Scene?.GetEntityById((int)moveInfo.EntityId);

        // Skip if entity doesn't exist or scene is still loading
        if (entity == null) return;
        // TODO: Check if player's scene load state is LOADING and skip if so

        var motionInfo = moveInfo.MotionInfo;
        var motionState = motionInfo.State;

        // Create and fire EntityMoveEvent
        var moveEvent = new EntityMoveEvent(
            entity,
            new Position(motionInfo.Pos),
            new Position(motionInfo.Rot),
            motionState);
        moveEvent.Call();

        // Move entity to new position/rotation
        entity.Move(moveEvent.Position, moveEvent.Rotation);

        // Update last move tracking
        entity.LastMoveSceneTimeMs = (int)moveInfo.SceneTime;
        entity.LastMoveReliableSeq = (int)moveInfo.ReliableSeq;
        entity.MotionState = motionState;

        // Handle stamina
        player.StaminaManager?.HandleCombatInvocationsNotify(moveInfo, entity);

        // Handle fall damage
        HandleFallDamage(player, entity, motionState, motionInfo);

        // Update entry's combat data if we modified the move (for forwarding)
        // Re-serialize the moveInfo in case position was modified by event
        if (moveEvent.Position != new Position(motionInfo.Pos))
        {
            var updatedMoveInfo = new EntityMoveInfo
            {
                EntityId = moveInfo.EntityId,
                MotionInfo = new MotionInfo
                {
                    Pos = moveEvent.Position.ToProto(),
                    Rot = moveEvent.Rotation.ToProto(),
                    State = motionState,
                    Speed = motionInfo.Speed
                },
                SceneTime = moveInfo.SceneTime,
                ReliableSeq = moveInfo.ReliableSeq
            };
            entry.CombatData = updatedMoveInfo.ToByteString();
        }
    }

    /// <summary>
    /// Handle landing damage based on cached LAND_SPEED and FALL_ON_GROUND state
    /// </summary>
    private void HandleFallDamage(PlayerInstance player, BaseEntity entity, MotionState motionState, MotionInfo motionInfo)
    {
        if (motionState == MotionState.LandSpeed)
        {
            // Cache the landing Y speed for later fall damage calculation
            _cachedLandingSpeed = motionInfo.Speed?.Y ?? 0;
            _cachedLandingTimeMillisecond = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            _monitorLandingEvent = true;
        }

        if (_monitorLandingEvent && motionState == MotionState.FallOnGround)
        {
            _monitorLandingEvent = false;
            ApplyFallDamage(player, entity, motionState);
        }
    }

    /// <summary>
    /// Calculate and apply fall damage
    /// </summary>
    private void ApplyFallDamage(PlayerInstance player, BaseEntity entity, MotionState motionState)
    {
        // Skip if player is in god mode
        // TODO: Check god mode

        // Dirty patch: prevent dying when talking to NPC after plunge attack
        // Check timing - if delay between landing and fall exceeds 200ms, skip
        long now = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        if (now - _cachedLandingTimeMillisecond > 200)
        {
            _cachedLandingSpeed = 0;
            return;
        }

        float currentHp = entity.GetFightProperty(FightProp.FIGHT_PROP_CUR_HP);
        float maxHp = entity.GetFightProperty(FightProp.FIGHT_PROP_MAX_HP);

        // Calculate damage factor based on landing speed
        float damageFactor = 0;
        if (_cachedLandingSpeed < -28)
            damageFactor = 1.0f;
        else if (_cachedLandingSpeed < -26.5)
            damageFactor = 0.66f;
        else if (_cachedLandingSpeed < -25)
            damageFactor = 0.5f;
        else if (_cachedLandingSpeed < -23.5)
            damageFactor = 0.33f;

        float damage = maxHp * damageFactor;
        float newHp = currentHp - damage;
        if (newHp < 0) newHp = 0;

        if (damageFactor > 0)
        {
            entity.SetFightProperty((int)FightProp.FIGHT_PROP_CUR_HP, newHp);
            entity.Scene.World.BroadcastPacket(
                new PacketEntityFightPropChangeReasonNotify(
                    entity, FightProp.FIGHT_PROP_CUR_HP, newHp, 0));

            // If dead, kill avatar with fall damage
            if (newHp == 0)
            {
                player.StaminaManager?.KillAvatar(entity, 1); // PLAYER_DIE_TYPE_FALL = 1
            }
        }

        _cachedLandingSpeed = 0;
    }

    /// <summary>
    /// Handle COMBAT_ANIMATOR_PARAMETER_CHANGED entries
    /// </summary>
    private void HandleAnimatorParameterChanged(CombatInvokeEntry entry)
    {
        var animatorInfo = EvtAnimatorParameterInfo.Parser.ParseFrom(entry.CombatData);

        if (animatorInfo.IsServerCache)
        {
            // Rebuild with isServerCache set to false for forwarding
            var updatedAnimatorInfo = new EvtAnimatorParameterInfo
            {
                EntityId = animatorInfo.EntityId,
                IsServerCache = false,
                NameId = animatorInfo.NameId,
                Value = animatorInfo.Value
            };
            entry.CombatData = updatedAnimatorInfo.ToByteString();
        }
    }

    /// <summary>
    /// Determine whether an entry should be forwarded to other players.
    /// Skips MOTION_STATE_NOTIFY and MOTION_STATE_FIGHT to avoid interrupting client animations.
    /// </summary>
    private bool ShouldForward(CombatInvokeEntry entry)
    {
        if (entry.ArgumentType != CombatTypeArgument.EntityMove)
            return true;

        var moveInfo = EntityMoveInfo.Parser.ParseFrom(entry.CombatData);
        var motionState = moveInfo.MotionInfo.State;

        if (motionState == MotionState.Notify ||
            motionState == MotionState.Fight)
            return false;

        return true;
    }
}
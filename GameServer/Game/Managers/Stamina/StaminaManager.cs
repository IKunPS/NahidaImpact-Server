using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.GameServer.Game.Worlds;
using NahidaImpact.GameServer.Server.Packet.Send.Entity;
using NahidaImpact.Prop;
using NahidaImpact.Util;

namespace NahidaImpact.GameServer.Game.Managers.Stamina;

public class StaminaManager : BasePlayerManager
{
    public const int GlobalCharacterMaximumStamina = 50000; // PlayerProperty.PROP_MAX_STAMINA.getMax()
    public const int GlobalVehicleMaxStamina = 10000;

    private static readonly Dictionary<string, HashSet<MotionState>> MotionStatesCategorized = new()
    {
        ["CLIMB"] = new()
        {
            MotionState.Climb,
            MotionState.StandbyToClimb
        },
        ["DASH"] = new()
        {
            MotionState.DangerDash,
            MotionState.Dash
        },
        ["FLY"] = new()
        {
            MotionState.Fly,
            MotionState.FlyFast,
            MotionState.FlySlow,
            MotionState.PoweredFly
        },
        ["RUN"] = new()
        {
            MotionState.DangerRun,
            MotionState.Run
        },
        ["SKIFF"] = new()
        {
            MotionState.SkiffNormal,
            MotionState.SkiffDash,
            MotionState.SkiffPoweredDash
        },
        ["STANDBY"] = new()
        {
            MotionState.DangerStandbyMove,
            MotionState.DangerStandby,
            MotionState.LadderToStandby,
            MotionState.StandbyMove,
            MotionState.Standby
        },
        ["SWIM"] = new()
        {
            MotionState.SwimIdle,
            MotionState.SwimDash,
            MotionState.SwimJump,
            MotionState.SwimMove
        },
        ["WALK"] = new()
        {
            MotionState.DangerWalk,
            MotionState.Walk
        },
        ["OTHER"] = new()
        {
            MotionState.ClimbJump,
            MotionState.DashBeforeShake,
            MotionState.Fight,
            MotionState.JumpUpWallForStandby,
            MotionState.Notify,
            MotionState.SitIdle,
            MotionState.Jump
        },
        ["NOCOST_NORECOVER"] = new()
        {
            MotionState.LadderSlip,
            MotionState.Slip,
            MotionState.FlyIdle
        },
        ["IGNORE"] = new()
        {
            MotionState.CrouchIdle,
            MotionState.CrouchMove,
            MotionState.CrouchRoll,
            MotionState.DestroyVehicle,
            MotionState.FallOnGround,
            MotionState.FollowRoute,
            MotionState.ForceSetPos,
            MotionState.GoUpstairs,
            MotionState.JumpOffWall,
            MotionState.LadderIdle,
            MotionState.LadderMove,
            MotionState.LandSpeed,
            MotionState.MoveFailAck,
            MotionState.None,
            MotionState.Num,
            MotionState.QuestForceDrag,
            MotionState.Reset,
            MotionState.StandbyToLadder,
            MotionState.Waterfall
        }
    };

    private static readonly HashSet<int> TalentMovements = new() { 10013, 10413 };
    private static readonly Dictionary<int, float> ClimbFoodReductionMap = new() { { 0, 0.8f } };
    private static readonly Dictionary<int, float> DashFoodReductionMap = new() { { 0, 0.8f } };
    private static readonly Dictionary<int, float> FlyFoodReductionMap = new() { { 0, 0.8f } };
    private static readonly Dictionary<int, float> SwimFoodReductionMap = new() { { 0, 0.8f } };
    private static readonly Dictionary<int, float> ClimbTalentReductionMap = new() { { 262301, 0.8f } };
    private static readonly Dictionary<int, float> FlyTalentReductionMap = new()
    {
        { 212301, 0.8f },
        { 222301, 0.8f }
    };
    private static readonly Dictionary<int, float> SwimTalentReductionMap = new()
    {
        { 242301, 0.8f },
        { 542301, 0.8f }
    };

    private readonly Logger _logger = new("StaminaManager");
    private readonly Dictionary<string, BeforeUpdateStaminaListener> _beforeUpdateStaminaListeners = new();
    private readonly Dictionary<string, AfterUpdateStaminaListener> _afterUpdateStaminaListeners = new();
    private Position _currentCoordinates = new(0, 0, 0);
    private Position _previousCoordinates = new(0, 0, 0);
    private MotionState _currentState = MotionState.Standby;
    private MotionState _previousState = MotionState.Standby;
    private Timer _staminaHandlerTimer;
    private bool _staminaHandlerRunning;
    private readonly object _handlerLock = new();
    private PlayerInstance _cachedPlayer;
    private BaseEntity _cachedEntity;
    public int StaminaRecoverDelay = 0;
    private long _lastCostStaminaTime = 0;
    private int _lastSkillId = 0;
    private int _lastSkillCasterId = 0;
    private bool _lastSkillFirstTick = true;
    private int _vehicleId = -1;
    private int _vehicleStamina = GlobalVehicleMaxStamina;
    private long _lastSwimDashTime = 0;
    private static readonly long SWIM_DASH_COOLDOWN = 500; // 500ms cooldown

    public StaminaManager(PlayerInstance player) : base(player)
    {
    }

    public void SetSkillCast(int skillId, int skillCasterId)
    {
        _lastSkillFirstTick = true;
        _lastSkillId = skillId;
        _lastSkillCasterId = skillCasterId;
    }

    public int GetMaxCharacterStamina()
    {
        return Player.GetProperty(PlayerProp.PROP_MAX_STAMINA);
    }

    public int GetCurrentCharacterStamina()
    {
        return Player.GetProperty(PlayerProp.PROP_CUR_PERSIST_STAMINA);
    }

    public int GetMaxVehicleStamina() => GlobalVehicleMaxStamina;

    public int GetCurrentVehicleStamina() => _vehicleStamina;

    public long GetLastCostStaminaTime() => _lastCostStaminaTime;

    public void SetLastCostStaminaTime(long time)
    {
        _lastCostStaminaTime = time;
    }

    public bool AddCurrentStamina(int amount)
    {
        var cur = GetCurrentCharacterStamina();
        var max = GetMaxCharacterStamina();
        if (cur >= max) return false;
        var value = cur + amount;
        if (value > max) value = max;
        Player.SetProperty(PlayerProp.PROP_CUR_PERSIST_STAMINA, value);
        return true;
    }

    public bool RegisterBeforeUpdateStaminaListener(string listenerName, BeforeUpdateStaminaListener listener)
    {
        return _beforeUpdateStaminaListeners.TryAdd(listenerName, listener);
    }

    public bool UnregisterBeforeUpdateStaminaListener(string listenerName)
    {
        return _beforeUpdateStaminaListeners.Remove(listenerName);
    }

    public bool RegisterAfterUpdateStaminaListener(string listenerName, AfterUpdateStaminaListener listener)
    {
        return _afterUpdateStaminaListeners.TryAdd(listenerName, listener);
    }

    public bool UnregisterAfterUpdateStaminaListener(string listenerName)
    {
        return _afterUpdateStaminaListeners.Remove(listenerName);
    }

    private bool IsPlayerMoving()
    {
        float diffX = _currentCoordinates.X - _previousCoordinates.X;
        float diffY = _currentCoordinates.Y - _previousCoordinates.Y;
        float diffZ = _currentCoordinates.Z - _previousCoordinates.Z;
        return Math.Abs(diffX) > 0.3 || Math.Abs(diffY) > 0.2 || Math.Abs(diffZ) > 0.3;
    }

    public int UpdateStaminaRelative(Consumption consumption, bool isCharacterStamina)
    {
        int currentStamina = isCharacterStamina ? GetCurrentCharacterStamina() : GetCurrentVehicleStamina();
        if (consumption.Amount == 0) return currentStamina;

        // Notify listeners
        foreach (var listener in _beforeUpdateStaminaListeners.Values)
        {
            var overridden = listener.OnBeforeUpdateStamina(consumption.Type.ToString(), consumption, isCharacterStamina);
            if (overridden.Type != consumption.Type && overridden.Amount != consumption.Amount)
            {
                _logger.Debug($"Stamina update relative({consumption.Type}, {consumption.Amount}) overridden by listener");
                return currentStamina;
            }
        }

        int maxStamina = isCharacterStamina ? GetMaxCharacterStamina() : GetMaxVehicleStamina();
        _logger.Debug($"{(isCharacterStamina ? "C" : "V")} {currentStamina}/{maxStamina}\t{_currentState}\t{(IsPlayerMoving() ? "moving" : "      ")}\t({consumption.Type},{consumption.Amount})");

        int newStamina = currentStamina + consumption.Amount;
        if (newStamina < 0) newStamina = 0;
        else if (newStamina > maxStamina) newStamina = maxStamina;

        return SetStamina(consumption.Type.ToString(), newStamina, isCharacterStamina);
    }

    public int UpdateStaminaAbsolute(string reason, int newStamina, bool isCharacterStamina)
    {
        int currentStamina = isCharacterStamina ? GetCurrentCharacterStamina() : GetCurrentVehicleStamina();

        foreach (var listener in _beforeUpdateStaminaListeners.Values)
        {
            int overridden = listener.OnBeforeUpdateStamina(reason, newStamina, isCharacterStamina);
            if (overridden != newStamina)
            {
                _logger.Debug($"Stamina update absolute({reason}, {newStamina}) overridden by listener");
                return currentStamina;
            }
        }

        int maxStamina = isCharacterStamina ? GetMaxCharacterStamina() : GetMaxVehicleStamina();
        if (newStamina < 0) newStamina = 0;
        else if (newStamina > maxStamina) newStamina = maxStamina;

        return SetStamina(reason, newStamina, isCharacterStamina);
    }

    public int SetStamina(string reason, int newStamina, bool isCharacterStamina)
    {
        // If stamina usage is disabled or player has unlimited stamina, keep max
        // TODO: Check GAME_OPTIONS.staminaUsage and Player.IsUnlimitedStamina()
        if (false) // Placeholder for config check
        {
            newStamina = GetMaxCharacterStamina();
        }

        if (isCharacterStamina)
        {
            Player.SetProperty(PlayerProp.PROP_CUR_PERSIST_STAMINA, newStamina);
        }
        else
        {
            _vehicleStamina = newStamina;
            // TODO: Send PacketVehicleStaminaNotify when created
        }

        // Notify listeners
        int finalStamina = newStamina;
        foreach (var listener in _afterUpdateStaminaListeners.Values)
        {
            listener.OnAfterUpdateStamina(reason, finalStamina, isCharacterStamina);
        }

        return newStamina;
    }

    // ===== Kill avatar =====

    public void KillAvatar(BaseEntity entity, uint dieType)
    {
        // TODO: Send PacketAvatarLifeStateChangeNotify with proper params
        // TODO: Send PacketLifeStateChangeNotify when created
        entity.SetFightProperty((int)FightProp.FIGHT_PROP_CUR_HP, 0);
        entity.Scene.World.BroadcastPacket(new PacketEntityFightPropChangeReasonNotify(
            entity, FightProp.FIGHT_PROP_CUR_HP, 0, 0));
        // TODO: Broadcast PacketLifeStateChangeNotify when created
        Player.Scene.RemoveEntity(entity);

        if (entity is EntityAvatar avatar)
        {
            avatar.SetKilled(dieType, 0);
            avatar.OnDeath(0);
        }
    }

    // ===== Sustained Stamina Handler =====

    public void StartSustainedStaminaHandler()
    {
        lock (_handlerLock)
        {
            if (_staminaHandlerRunning) return;
            _staminaHandlerRunning = true;

            _staminaHandlerTimer = new Timer(_ =>
            {
                SustainedStaminaHandlerTick();
            }, null, 0, 200); // Run every 200ms
        }
    }

    public void StopSustainedStaminaHandler()
    {
        lock (_handlerLock)
        {
            if (!_staminaHandlerRunning) return;
            _staminaHandlerRunning = false;
            _staminaHandlerTimer?.Dispose();
            _staminaHandlerTimer = null;
            _logger.Debug("SustainedStaminaHandler stopped");
        }
    }

    private void SustainedStaminaHandlerTick()
    {
        if (Player == null || Player.TeamManager == null || Player.Scene == null || Player.World == null)
        {
            StopSustainedStaminaHandler();
            return;
        }
        try
        {
            bool moving = IsPlayerMoving();
            int currentCharacterStamina = GetCurrentCharacterStamina();
            int maxCharacterStamina = GetMaxCharacterStamina();
            int currentVehicleStamina = GetCurrentVehicleStamina();
            int maxVehicleStamina = GetMaxVehicleStamina();

            if (moving || (currentCharacterStamina < maxCharacterStamina) || (currentVehicleStamina < maxVehicleStamina))
            {
                _logger.Debug($"Player moving: {moving}, stamina full: {currentCharacterStamina >= maxCharacterStamina}, recalculate stamina");
                bool isCharacterStamina = true;
                Consumption consumption;

                if (MotionStatesCategorized["CLIMB"].Contains(_currentState))
                    consumption = GetClimbConsumption();
                else if (MotionStatesCategorized["DASH"].Contains(_currentState))
                    consumption = GetDashConsumption();
                else if (MotionStatesCategorized["FLY"].Contains(_currentState))
                    consumption = GetFlyConsumption();
                else if (MotionStatesCategorized["RUN"].Contains(_currentState))
                    consumption = new Consumption(ConsumptionType.RUN);
                else if (MotionStatesCategorized["SKIFF"].Contains(_currentState))
                {
                    consumption = GetSkiffConsumption();
                    isCharacterStamina = false;
                }
                else if (MotionStatesCategorized["STANDBY"].Contains(_currentState))
                    consumption = new Consumption(ConsumptionType.STANDBY);
                else if (MotionStatesCategorized["SWIM"].Contains(_currentState))
                    consumption = GetSwimConsumptions();
                else if (MotionStatesCategorized["WALK"].Contains(_currentState))
                    consumption = new Consumption(ConsumptionType.WALK);
                else if (MotionStatesCategorized["NOCOST_NORECOVER"].Contains(_currentState))
                    consumption = new Consumption();
                else if (MotionStatesCategorized["OTHER"].Contains(_currentState))
                    consumption = GetOtherConsumptions();
                else // IGNORE
                    return;

                // Do not apply reduction factor when recovering stamina
                if (consumption.Amount < 0 && isCharacterStamina)
                {
                    // TODO: Check team resonances for 10301
                    // if (player.TeamManager.GetTeamResonances().Contains(10301))
                    //     consumption.Amount = (int)(consumption.Amount * 0.85f);
                }

                // Delay 1 second before starts recovering stamina
                if (consumption.Amount != 0)
                {
                    if (consumption.Amount < 0)
                    {
                        StaminaRecoverDelay = 0;
                    }
                    if (consumption.Amount > 0
                        && consumption.Type != ConsumptionType.POWERED_FLY
                        && consumption.Type != ConsumptionType.POWERED_SKIFF)
                    {
                        if (StaminaRecoverDelay < 5)
                        {
                            StaminaRecoverDelay++;
                            consumption.Amount = 0;
                            _logger.Debug($"Delaying recovery: {StaminaRecoverDelay}");
                        }
                    }
                    UpdateStaminaRelative(consumption, isCharacterStamina);
                }
            }
            _previousState = _currentState;
            _previousCoordinates = _currentCoordinates.Clone();
        }
        catch (Exception e)
        {
            _logger.Error($"Stamina handler tick failed uid:{Player?.Uid}", e);
        }
    }

    // ===== Main handler called from combat invocations =====

    public void HandleCombatInvocationsNotify(EntityMoveInfo moveInfo, BaseEntity entity)
    {
        _cachedPlayer = Player;
        _cachedEntity = entity;

        var motionInfo = moveInfo.MotionInfo;
        var motionState = motionInfo.State;
        var notifyEntityId = moveInfo.EntityId;
        var currentAvatarEntityId = Player.TeamManager?.GetCurrentAvatarEntity()?.Id ?? 0;
        if (notifyEntityId != currentAvatarEntityId && notifyEntityId != _vehicleId)
        {
            return;
        }

        // Update previous motion state
        _previousState = _currentState;

        // Update current state
        _currentState = motionState;

        Vector posVector = motionInfo.Pos;
        Position newPos = new Position(posVector.X, posVector.Y, posVector.Z);
        if (newPos.X != 0 || newPos.Y != 0 || newPos.Z != 0)
        {
            _currentCoordinates = newPos;
        }

        StartSustainedStaminaHandler();
        HandleImmediateStamina(motionState);
    }

    public void HandleVehicleInteractReq(int vehicleId, int vehicleInteractType)
    {
        // VEHICLE_INTERACT_TYPE_IN = 0
        if (vehicleInteractType == 0)
        {
            _vehicleId = vehicleId;
            UpdateStaminaAbsolute("board vehicle", GetMaxCharacterStamina(), true);
            UpdateStaminaAbsolute("board vehicle", GetMaxVehicleStamina(), false);
        }
        else
        {
            _vehicleId = -1;
        }
    }

    public void HandleEvtDoSkillSuccNotify(int skillId, int casterId)
    {
        if (casterId != (Player.TeamManager?.GetCurrentAvatarEntity()?.Id ?? 0))
            return;

        SetSkillCast(skillId, casterId);
    }

    public void HandleMixinCostStamina(bool isSwim)
    {
        if (_lastSkillCasterId == (Player.TeamManager?.GetCurrentAvatarEntity()?.Id ?? 0))
        {
            HandleImmediateStamina(_lastSkillId);
        }
    }

    // ===== Immediate stamina handling =====

    private void HandleImmediateStamina(MotionState motionState)
    {
        // Don't double dip on sustained stamina start costs
        if (_previousState == _currentState)
            return;

        switch (motionState)
        {
            case MotionState.Climb:
                UpdateStaminaRelative(new Consumption(ConsumptionType.CLIMB_START), true);
                break;
            case MotionState.DashBeforeShake:
                UpdateStaminaRelative(new Consumption(ConsumptionType.SPRINT), true);
                break;
            case MotionState.ClimbJump:
                UpdateStaminaRelative(new Consumption(ConsumptionType.CLIMB_JUMP), true);
                break;
            case MotionState.SwimDash:
                {
                    long currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    if (currentTime - _lastSwimDashTime >= SWIM_DASH_COOLDOWN)
                    {
                        UpdateStaminaRelative(new Consumption(ConsumptionType.SWIM_DASH_START), true);
                        _lastSwimDashTime = currentTime;
                        _logger.Debug($"Swim dash triggered, cooldown: {SWIM_DASH_COOLDOWN}ms");
                    }
                    else
                    {
                        long remainingCooldown = SWIM_DASH_COOLDOWN - (currentTime - _lastSwimDashTime);
                        _logger.Debug($"Swim dash on cooldown, remaining: {remainingCooldown}ms");
                    }
                }
                break;
        }
    }

    private void HandleImmediateStamina(int skillId)
    {
        Consumption consumption = GetFightConsumption(skillId);
        UpdateStaminaRelative(consumption, true);
    }

    private void HandleDrowning()
    {
        int stamina = GetCurrentCharacterStamina();
        if (stamina < 10)
        {
            _logger.Debug($"{GetCurrentCharacterStamina()}/{GetMaxCharacterStamina()}\t{_currentState}");
            if (_currentState != MotionState.SwimIdle)
            {
                KillAvatar(_cachedEntity, 0); // PLAYER_DIE_TYPE_DRAWN = 0 placeholder
            }
        }
    }

    // ===== Consumption calculators =====

    public Consumption GetFightConsumption(int skillCasting)
    {
        // Talent moving
        if (TalentMovements.Contains(skillCasting))
        {
            return GetTalentMovingSustainedCost(skillCasting);
        }
        return new Consumption();
    }

    private Consumption GetClimbConsumption()
    {
        Consumption consumption = new();
        if (_currentState == MotionState.Climb && IsPlayerMoving())
        {
            consumption.Type = ConsumptionType.CLIMBING;
            consumption.Amount = (int)ConsumptionType.CLIMBING;
        }
        consumption.Amount = (int)(consumption.Amount * GetFoodCostReductionFactor(ClimbFoodReductionMap));
        consumption.Amount = (int)(consumption.Amount * GetTalentCostReductionFactor(ClimbTalentReductionMap));
        return consumption;
    }

    private Consumption GetSwimConsumptions()
    {
        HandleDrowning();
        Consumption consumption = new();
        if (_currentState == MotionState.SwimMove)
        {
            consumption.Type = ConsumptionType.SWIMMING;
            consumption.Amount = (int)ConsumptionType.SWIMMING;
        }
        if (_currentState == MotionState.SwimDash)
        {
            consumption.Type = ConsumptionType.SWIM_DASH;
            consumption.Amount = (int)ConsumptionType.SWIM_DASH;

            // Skip sustained cost if just switched to SWIM_DASH
            // (handleImmediateStamina already handled SWIM_DASH_START)
            if (_previousState != MotionState.SwimDash)
            {
                consumption.Amount = 0;
            }
        }
        consumption.Amount = (int)(consumption.Amount * GetFoodCostReductionFactor(SwimFoodReductionMap));
        consumption.Amount = (int)(consumption.Amount * GetTalentCostReductionFactor(SwimTalentReductionMap));
        return consumption;
    }

    private Consumption GetDashConsumption()
    {
        Consumption consumption = new();
        if (_currentState == MotionState.Dash)
        {
            consumption.Type = ConsumptionType.DASH;
            consumption.Amount = (int)ConsumptionType.DASH;
            consumption.Amount = (int)(consumption.Amount * GetFoodCostReductionFactor(DashFoodReductionMap));
        }
        return consumption;
    }

    private Consumption GetFlyConsumption()
    {
        if (_currentState == MotionState.PoweredFly)
        {
            return new Consumption(ConsumptionType.POWERED_FLY);
        }
        Consumption consumption = new(ConsumptionType.FLY);
        consumption.Amount = (int)(consumption.Amount * GetFoodCostReductionFactor(FlyFoodReductionMap));
        consumption.Amount = (int)(consumption.Amount * GetTalentCostReductionFactor(FlyTalentReductionMap));
        return consumption;
    }

    private Consumption GetSkiffConsumption()
    {
        return _currentState switch
        {
            MotionState.SkiffDash => new Consumption(ConsumptionType.SKIFF_DASH),
            MotionState.SkiffPoweredDash => new Consumption(ConsumptionType.POWERED_SKIFF),
            MotionState.SkiffNormal => new Consumption(ConsumptionType.SKIFF),
            _ => new Consumption()
        };
    }

    private Consumption GetOtherConsumptions()
    {
        return _currentState switch
        {
            MotionState.Fight => new Consumption(ConsumptionType.FIGHT, 500),
            _ => new Consumption()
        };
    }

    // ===== Reduction factors =====

    private float GetTalentCostReductionFactor(Dictionary<int, float> talentReductionMap)
    {
        float reduction = 1;
        // TODO: Check team active members' proud skills for reductions
        return reduction;
    }

    private float GetFoodCostReductionFactor(Dictionary<int, float> foodReductionMap)
    {
        float reduction = 1;
        // TODO: Check consumed food buffs
        return reduction;
    }

    // ===== Special consumption types =====

    private Consumption GetTalentMovingSustainedCost(int skillId)
    {
        if (_lastSkillFirstTick)
        {
            _lastSkillFirstTick = false;
            return new Consumption(ConsumptionType.TALENT_DASH, -1000);
        }
        else
        {
            return new Consumption(ConsumptionType.TALENT_DASH, -500);
        }
    }
}

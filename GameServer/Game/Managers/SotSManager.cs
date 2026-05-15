using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.Util;
using Timer = System.Timers.Timer;

namespace NahidaImpact.GameServer.Game.Managers;

public class SotSManager(PlayerInstance player) : BasePlayerManager(player)
{
    private Timer? _autoRecoverTimer;
    private readonly Logger _logger = new("SotSManager");

    // Current statue point info
    public int CurrentSceneId { get; private set; }
    public int CurrentPointId { get; private set; }
    
    public void HandleEnterTransPointRegionNotify(int sceneId, int pointId)
    {
        _logger.Debug($"Player entered statue region: scene={sceneId}, point={pointId}");
        CurrentSceneId = sceneId;
        CurrentPointId = pointId;

        // TODO: Implement auto-revive of dead team members
        // Requires: TeamManager.GetActiveTeam(), EntityAvatar.IsAlive(), TeamManager.ReviveAvatar(), TeamManager.HealAvatar()
        // AutoRevive();

        if (_autoRecoverTimer == null)
        {
            _autoRecoverTimer = new Timer(15000); // 15 seconds interval
            _autoRecoverTimer.Elapsed += OnAutoRecoverTick;
            _autoRecoverTimer.AutoReset = true;
            _autoRecoverTimer.Start();
        }
    }
    
    public void HandleExitTransPointRegionNotify()
    {
        _logger.Debug("Player left statue region");
        DestroyTimerThread();
    }

    private void DestroyTimerThread()
    {
        if (_autoRecoverTimer != null)
        {
            _autoRecoverTimer.Stop();
            _autoRecoverTimer.Dispose();
            _autoRecoverTimer = null;
        }
    }

    /// <summary>
    /// Periodic healing tick. Heals all active team members.
    /// TODO: Implement full healing with spring volume management like Java.
    /// </summary>
    private void OnAutoRecoverTick(object? sender, System.Timers.ElapsedEventArgs e)
    {
        // TODO: Implement healing with spring volume refill logic from Java
        // Requires: TeamManager.GetActiveTeam(), EntityAvatar.GetFightProperty(), FightPropType enum
        // For now this is a stub that just keeps the timer alive
    }
}

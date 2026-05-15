using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.GameServer.Game.Worlds;

namespace NahidaImpact.GameServer.Server.Event.Player;

public class PlayerMoveEvent
{
    public enum MoveType
    {
        /// <summary>The player has sent a combat invocation to move.</summary>
        PLAYER,
        /// <summary>The server has requested that the player moves.</summary>
        SERVER
    }

    public PlayerInstance Player { get; }
    public MoveType Type { get; }
    public Position From { get; }
    public Position To { get; }
    public bool IsCancelled { get; private set; }

    public PlayerMoveEvent(PlayerInstance player, MoveType type, Position from, Position to)
    {
        Player = player;
        Type = type;
        From = from;
        To = to;
    }

    public void Cancel()
    {
        IsCancelled = true;
    }

    public Position GetDestination() => To;

    /// <summary>
    /// Fire the event. Returns false if cancelled.
    /// </summary>
    public bool Call()
    {
        // TODO: Hook into global event bus when implemented
        return !IsCancelled;
    }
}

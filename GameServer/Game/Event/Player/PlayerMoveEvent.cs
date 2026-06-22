using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.GameServer.Game.Worlds;

namespace NahidaImpact.GameServer.Game.Event.Player;

public class PlayerMoveEvent
{
    public enum MoveType
    {
        PLAYER,
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

    public bool Call()
    {
        return !IsCancelled;
    }
}
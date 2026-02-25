using NahidaImpact.GameServer.Game.Player;

namespace NahidaImpact.GameServer.Game;

public class BasePlayerManager(PlayerInstance player)
{
    public PlayerInstance Player { get; private set; } = player;
}
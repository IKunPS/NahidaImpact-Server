using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.GameServer.Game.Player;

namespace NahidaImpact.GameServer.Game.Event.Player;

public class PlayerTeamDeathEvent
{
    public PlayerInstance Player { get; }
    public EntityAvatar DiedAvatar { get; }

    public PlayerTeamDeathEvent(PlayerInstance player, EntityAvatar diedAvatar)
    {
        Player = player;
        DiedAvatar = diedAvatar;
    }

    public void Call()
    {
    }
}
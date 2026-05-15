using NahidaImpact.Database.Avatar;
using NahidaImpact.GameServer.Game.Player;

namespace NahidaImpact.GameServer.Game.Managers;

public class SatiationManager(PlayerInstance player) : BasePlayerManager(player)
{
    public void RemoveSatiationDirectly(AvatarDataInfo avatar, int amount)
    {
        // TODO: Implement satiation system
    }
}

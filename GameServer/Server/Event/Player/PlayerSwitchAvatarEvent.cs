using NahidaImpact.Database.Avatar;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.GameServer.Game.Player;

namespace NahidaImpact.GameServer.Server.Event.Player;

public class PlayerSwitchAvatarEvent
{
    public PlayerInstance Player { get; }
    public AvatarDataInfo PreviousAvatar { get; }
    public AvatarDataInfo NewAvatar { get; set; }
    public EntityAvatar? NewAvatarEntity { get; set; }

    public PlayerSwitchAvatarEvent(PlayerInstance player, AvatarDataInfo previousAvatar, AvatarDataInfo newAvatar)
    {
        Player = player;
        PreviousAvatar = previousAvatar;
        NewAvatar = newAvatar;
    }

    public bool Call()
    {
        // TODO: Implement event system
        // Event handler callbacks would go here
        return true;
    }
}

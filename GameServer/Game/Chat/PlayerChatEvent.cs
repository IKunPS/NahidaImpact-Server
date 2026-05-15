using NahidaImpact.GameServer.Game.Player;

namespace NahidaImpact.GameServer.Game.Chat;

public class PlayerChatEvent
{
    public PlayerInstance Sender { get; }
    public string Message { get; set; }
    public PlayerInstance? Target { get; set; }
    public int? ChannelId { get; set; }
    public bool IsCanceled { get; set; }

    // Private chat - text
    public PlayerChatEvent(PlayerInstance player, string message, PlayerInstance? target)
    {
        Sender = player;
        Message = message;
        Target = target;
    }

    // Private chat - emote
    public PlayerChatEvent(PlayerInstance player, int emoteId, PlayerInstance? target)
    {
        Sender = player;
        Message = emoteId.ToString();
        Target = target;
    }

    // Team/World chat - text
    public PlayerChatEvent(PlayerInstance player, string message, int channelId)
    {
        Sender = player;
        Message = message;
        ChannelId = channelId;
    }

    // Team/World chat - icon
    public PlayerChatEvent(PlayerInstance player, int icon, int channelId)
    {
        Sender = player;
        Message = icon.ToString();
        ChannelId = channelId;
    }
    
    public int GetTargetUid() => Target?.Uid ?? -1;

    public int GetMessageAsInt()
    {
        return int.TryParse(Message, out var v) ? v : -1;
    }
    
    public int GetChannel() => ChannelId ?? -1;
}
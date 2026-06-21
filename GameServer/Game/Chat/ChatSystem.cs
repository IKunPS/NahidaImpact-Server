using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NahidaImpact.GameServer.Command;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.GameServer.Server.Packet.Send.Player;
using NahidaImpact.Proto;
using NahidaImpact.Util;

namespace NahidaImpact.GameServer.Game.Chat;

public class ChatSystem
{
    public static ChatSystem Instance { get; } = new();

    private static readonly Regex CommandPrefixRegex = new(@"^[/!]");
    private static readonly Regex CommandsRegex = new(@"\n[/!]");
    
    public static string WelcomeMessage { get; set; } = "Welcome to Nahida Impact Server!";
    public static int[] WelcomeEmotes { get; set; } = { 2007, 1002, 4010 };
    
    private readonly Dictionary<int, Dictionary<int, List<ChatInfo>>> _history = new();
    
    private bool TryInvokeCommand(PlayerInstance sender, PlayerInstance? target, string rawMessage)
    {
        if (string.IsNullOrEmpty(rawMessage)) return false;
        if (!CommandPrefixRegex.IsMatch(rawMessage[..1])) return false;

        foreach (var line in CommandsRegex.Split(rawMessage[1..]))
        {
            _ = CommandManager.HandleCommand(line.Trim(), new PlayerCommandSender(sender));
        }

        return true;
    }
    
    private void PutInHistory(int uid, int partnerUid, ChatInfo info)
    {
        if (!_history.TryGetValue(uid, out var partnerMap))
        {
            partnerMap = new Dictionary<int, List<ChatInfo>>();
            _history[uid] = partnerMap;
        }

        if (!partnerMap.TryGetValue(partnerUid, out var list))
        {
            list = new List<ChatInfo>();
            partnerMap[partnerUid] = list;
        }

        list.Add(info);
    }

    public void ClearHistoryOnLogout(PlayerInstance player)
    {
        _history.Remove(player.Uid);
    }

    public void HandlePullPrivateChatReq(PlayerInstance player, int partnerUid)
    {
        if (!_history.TryGetValue(player.Uid, out var partnerMap))
        {
            partnerMap = new Dictionary<int, List<ChatInfo>>();
            _history[player.Uid] = partnerMap;
        }

        if (!partnerMap.TryGetValue(partnerUid, out var chatHistory))
        {
            chatHistory = new List<ChatInfo>();
            partnerMap[partnerUid] = chatHistory;
        }

        _ = player.SendPacket(new PacketPullPrivateChatRsp(chatHistory));
    }

    public void HandlePullRecentChatReq(PlayerInstance player)
    {
        // If this user has no chat history with server yet, send welcome messages
        if (!_history.TryGetValue(player.Uid, out var partnerMap))
        {
            partnerMap = new Dictionary<int, List<ChatInfo>>();
            _history[player.Uid] = partnerMap;
        }

        if (!partnerMap.ContainsKey(GameConstants.SERVER_CONSOLE_UID))
        {
            SendServerWelcomeMessages(player);
        }

        // Return the last 3 messages from server for the recent chat list
        var serverMessages = partnerMap.GetValueOrDefault(GameConstants.SERVER_CONSOLE_UID, new List<ChatInfo>());
        int historyLength = serverMessages.Count;
        var messages = serverMessages.Skip(Math.Max(historyLength - 3, 0)).Take(3).ToList();
        _ = player.SendPacket(new PacketPullRecentChatRsp(messages));
    }

    public async Task SendPrivateMessageFromServer(int targetUid, string message)
    {
        if (string.IsNullOrEmpty(message)) return;

        var target = PlayerInstance.GetPlayerInstanceByUid((uint)targetUid);
        if (target == null) return;

        var packet = new PacketPrivateChatNotify(GameConstants.SERVER_CONSOLE_UID, targetUid, message);
        PutInHistory(targetUid, GameConstants.SERVER_CONSOLE_UID, packet.ChatInfo);

        await target.SendPacket(packet);
    }

    public async Task SendPrivateMessageFromServer(int targetUid, int emote)
    {
        var target = PlayerInstance.GetPlayerInstanceByUid((uint)targetUid);
        if (target == null) return;

        var packet = new PacketPrivateChatNotify(GameConstants.SERVER_CONSOLE_UID, targetUid, emote);
        PutInHistory(targetUid, GameConstants.SERVER_CONSOLE_UID, packet.ChatInfo);

        await target.SendPacket(packet);
    }

    public async Task SendPrivateMessage(PlayerInstance player, int targetUid, string message)
    {
        if (string.IsNullOrEmpty(message)) return;

        // Get target (allow SERVER_CONSOLE_UID even if no online player)
        var target = PlayerInstance.GetPlayerInstanceByUid((uint)targetUid);
        if (target == null && targetUid != GameConstants.SERVER_CONSOLE_UID) return;

        // Invoke chat event (mirrors Java PlayerChatEvent)
        var chatEvent = new PlayerChatEvent(player, message, target);
        if (chatEvent.IsCanceled) return;

        // Fetch potentially modified target
        if (targetUid != GameConstants.SERVER_CONSOLE_UID)
        {
            targetUid = chatEvent.GetTargetUid();
            if (targetUid == -1) return;
            target = PlayerInstance.GetPlayerInstanceByUid((uint)targetUid);
        }

        // Fetch potentially modified message
        message = chatEvent.Message;
        if (string.IsNullOrEmpty(message)) return;

        // Create chat packet
        var packet = new PacketPrivateChatNotify(player.Uid, targetUid, message);

        // Send to sender and put in history
        await player.SendPacket(packet);
        PutInHistory(player.Uid, targetUid, packet.ChatInfo);

        // Check if command AFTER sending (mirrors Java order)
        bool isCommand = TryInvokeCommand(player, target, message);

        // Forward to target if not a command
        if (target != null && !isCommand)
        {
            await target.SendPacket(packet);
            PutInHistory(targetUid, player.Uid, packet.ChatInfo);
        }
    }
    
    public async Task SendPrivateMessage(PlayerInstance player, int targetUid, int emote)
    {
        // Get target
        var target = PlayerInstance.GetPlayerInstanceByUid((uint)targetUid);
        if (target == null && targetUid != GameConstants.SERVER_CONSOLE_UID) return;

        // Invoke chat event
        var chatEvent = new PlayerChatEvent(player, emote, target);
        if (chatEvent.IsCanceled) return;

        // Fetch potentially modified target
        if (targetUid != GameConstants.SERVER_CONSOLE_UID)
        {
            targetUid = chatEvent.GetTargetUid();
            if (targetUid == -1) return;
            target = PlayerInstance.GetPlayerInstanceByUid((uint)targetUid);
        }

        // Fetch potentially modified emote
        emote = chatEvent.GetMessageAsInt();
        if (emote == -1) return;

        // Create chat packet
        var packet = new PacketPrivateChatNotify(player.Uid, targetUid, emote);

        // Send to sender and put in history
        await player.SendPacket(packet);
        PutInHistory(player.Uid, targetUid, packet.ChatInfo);

        // Forward to target
        if (target != null)
        {
            await target.SendPacket(packet);
            PutInHistory(targetUid, player.Uid, packet.ChatInfo);
        }
    }

    public async Task SendTeamMessage(PlayerInstance player, int channel, string message)
    {
        if (string.IsNullOrEmpty(message)) return;

        // Check if command
        if (TryInvokeCommand(player, null, message)) return;

        // Invoke chat event
        var chatEvent = new PlayerChatEvent(player, message, channel);
        if (chatEvent.IsCanceled) return;

        // Fetch potentially modified message
        message = chatEvent.Message;
        if (string.IsNullOrEmpty(message)) return;

        // Fetch potentially modified channel
        channel = chatEvent.GetChannel();
        if (channel == -1) return;

        // Broadcast to world
        player.World?.BroadcastPacket(new PacketPlayerChatNotify(player, channel, message));
        await System.Threading.Tasks.Task.CompletedTask;
    }

    public async Task SendTeamMessage(PlayerInstance player, int channel, int icon)
    {
        // Invoke chat event
        var chatEvent = new PlayerChatEvent(player, icon, channel);
        if (chatEvent.IsCanceled) return;

        // Fetch potentially modified icon
        icon = chatEvent.GetMessageAsInt();
        if (icon == -1) return;

        // Fetch potentially modified channel
        channel = chatEvent.GetChannel();
        if (channel == -1) return;

        // Broadcast to world
        player.World?.BroadcastPacket(new PacketPlayerChatNotify(player, channel, icon));
        await System.Threading.Tasks.Task.CompletedTask;
    }
    
    private void SendServerWelcomeMessages(PlayerInstance player)
    {
        if (WelcomeEmotes is { Length: > 0 })
        {
            var random = new Random();
            _ = SendPrivateMessageFromServer(player.Uid, WelcomeEmotes[random.Next(WelcomeEmotes.Length)]);
        }

        if (!string.IsNullOrEmpty(WelcomeMessage))
        {
            _ = SendPrivateMessageFromServer(player.Uid, WelcomeMessage);
        }
    }
}
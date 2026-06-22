using NahidaImpact.GameServer.Server.Packet.Send.Ability;
using NahidaImpact.Util;

namespace NahidaImpact.GameServer.Game.Player;

public class CombatInvokeHandler
{
    private static readonly Logger Logger = new("CombatInvoke");
    private readonly PlayerInstance _player;
    private readonly List<CombatInvokeEntry> _entryListForwardAll = new();
    private readonly List<CombatInvokeEntry> _entryListForwardAllExceptCur = new();
    private readonly List<CombatInvokeEntry> _entryListForwardHost = new();
    private readonly object _lock = new();

    public CombatInvokeHandler(PlayerInstance player)
    {
        _player = player;
    }

    public void AddEntry(ForwardType forward, CombatInvokeEntry entry)
    {
        lock (_lock)
        {
            switch (forward)
            {
                case ForwardType.ForwardToAll:
                    _entryListForwardAll.Add(entry);
                    break;
                case ForwardType.ForwardToAllExceptCur:
                case ForwardType.ForwardToAllExistExceptCur:
                    _entryListForwardAllExceptCur.Add(entry);
                    break;
                case ForwardType.ForwardToHost:
                    _entryListForwardHost.Add(entry);
                    break;
            }
        }
    }

    public void Send()
    {
        if (_player.World == null || _player.Scene == null)
        {
            lock (_lock)
            {
                _entryListForwardAll.Clear();
                _entryListForwardAllExceptCur.Clear();
                _entryListForwardHost.Clear();
            }
            return;
        }

        lock (_lock)
        {
            try
            {
                if (_entryListForwardAll.Count > 0)
                {
                    var packet = new PacketCombatInvocationsNotify(_entryListForwardAll);
                    _player.Scene.BroadcastPacket(packet);
                    _entryListForwardAll.Clear();
                }
                if (_entryListForwardAllExceptCur.Count > 0)
                {
                    var packet = new PacketCombatInvocationsNotify(_entryListForwardAllExceptCur);
                    _player.Scene.BroadcastPacketToOthers(_player, packet);
                    _entryListForwardAllExceptCur.Clear();
                }
                if (_entryListForwardHost.Count > 0)
                {
                    var packet = new PacketCombatInvocationsNotify(_entryListForwardHost);
                    _player.World.Host?.SendPacket(packet);
                    _entryListForwardHost.Clear();
                }
            }
            catch (Exception ex)
            {
                Logger.Debug($"CombatInvoke send failed: {ex.Message}");
            }
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            _entryListForwardAll.Clear();
            _entryListForwardAllExceptCur.Clear();
            _entryListForwardHost.Clear();
        }
    }
}

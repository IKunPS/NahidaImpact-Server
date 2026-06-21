using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.GameServer.Server.Packet;
using NahidaImpact.Internationalization;
using NahidaImpact.KcpSharp;
using NahidaImpact.KcpSharp.Base;
using NahidaImpact.Util;
using NahidaImpact.Util.Extensions;
using NahidaImpact.Util.Security;
using System.Buffers;
using System.Net;
using NahidaImpact.Database.Account;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server;

public class Connection(KcpConversation conversation, IPEndPoint remote) : KcpConnection(conversation, remote)
{
    private static readonly Logger Logger = new("GameServer");

    public PlayerInstance? Player { get; set; }

    public AccountData Account { get; set; }

    private static readonly HashSet<string> DummyPacketNames =
    [

    ];

    public override async void Start()
    {
        Logger.Info(I18NManager.Translate("Server.ConnectionInfo.NewConnection", RemoteEndPoint.ToString()));
        State = SessionStateEnum.WAITING_FOR_TOKEN;
        try
        {
            await ReceiveLoop();
        }
        catch (Exception ex)
        {
            Logger.Error($"ReceiveLoop crashed: {ex}");
        }
    }

    public override void Stop(bool isServerStop = false)
    {
        Player?.OnLogoutAsync();
        KcpListener.UnregisterConnection(this);
        base.Stop(isServerStop);
    }

    protected async Task ReceiveLoop()
    {
        while (!CancelToken.IsCancellationRequested)
        {
            // WaitToReceiveAsync call completes when there is at least one message is received or the transport is closed.
            var result = await Conversation.WaitToReceiveAsync(CancelToken.Token);
            if (result.TransportClosed)
            {
                Logger.Debug(I18NManager.Translate("Server.ConnectionInfo.ConnectionClosed"));
                break;
            }
            
            if (result.BytesReceived > KcpListener.MAX_MSG_SIZE)
            {
                // The message is too large.
                Logger.Error(I18NManager.Translate("Server.ConnectionInfo.PacketTooLarge"));
                Conversation.SetTransportClosed();
                break;
            }
    
            var buffer = ArrayPool<byte>.Shared.Rent(result.BytesReceived);
            try
            {
                // TryReceive should not return false here, unless the transport is closed.
                // So we don't need to check for result.TransportClosed.
                if (!Conversation.TryReceive(buffer, out result))
                {
                    Logger.Error(I18NManager.Translate("Server.ConnectionInfo.FailedToReceive"));
                    break;
                }
                
                await ProcessMessageAsync(buffer.AsMemory(0, result.BytesReceived));
            }
            catch (Exception ex)
            {
                Logger.Error(I18NManager.Translate("Server.ConnectionInfo.ParseError"), ex);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
    
        Stop();
    }

    // DO THE PROCESSING OF THE GAME PACKET
    private async Task ProcessMessageAsync(Memory<byte> data)
    {
        byte[] gamePacket = data.ToArray();
        Crypto.Xor(gamePacket, UseSecretKey ? SecretKey : Crypto.DISPATCH_KEY);
    
        await using MemoryStream ms = new(gamePacket);
        using BinaryReader br = new(ms);
        
        // Handle
        try
        {
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                // Length
                if (br.BaseStream.Length - br.BaseStream.Position < 12) return;
                
                // Packet sanity check
                var headMagic = br.ReadUInt16BE();
                if (headMagic != 0x4567)
                {
                    Logger.Error(I18NManager.Translate("Server.ConnectionInfo.BadDataPackage", headMagic.ToString("X")));
                    return;
                }

                var cmdId = br.ReadUInt16BE();
                var headerLength = br.ReadUInt16BE();
                var bodyLength = br.ReadUInt32BE();

                // Data
                var header = br.ReadBytes(headerLength);
                var body = br.ReadBytes((int)bodyLength);

                var tail = br.ReadUInt16BE();
                if (tail != 0x89AB)
                {
                    Logger.Error(I18NManager.Translate("Server.ConnectionInfo.InvalidFooter", tail.ToString("X")));
                    return;
                }

                LogPacket("Recv", cmdId, body);
                await HandlePacket(cmdId, header, body);
            }
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
        }
    }

    private async Task HandlePacket(ushort opcode, byte[] header, byte[] payload)
    {
        var packetName = LogMap.GetValueOrDefault(opcode);
        if (DummyPacketNames.Contains(packetName!))
        {
            await SendDummy(packetName!);
            Logger.Info(I18NManager.Translate("Server.ConnectionInfo.DummySend", packetName));
            return;
        }

        // Find the Handler for this opcode
        var handler = HandlerManager.GetHandler(opcode);
        if (handler != null)
        {
            // Handle
            // Make sure session is ready for packets
            var state = State;
            switch (opcode)
            {
                case CmdIds.GetPlayerTokenReq:
                {
                    if (state != SessionStateEnum.WAITING_FOR_TOKEN)
                    {
                        return;
                    }
                    goto default;
                }
                case CmdIds.PlayerLoginReq:
                {
                    if (state != SessionStateEnum.WAITING_FOR_LOGIN)
                    {
                        return;
                    }
                    goto default;
                }
                case CmdIds.SetPlayerBornDataReq:
                {
                    if (state != SessionStateEnum.PICKING_CHARACTER)
                    {
                        return;
                    }
                    goto default;
                }
                default:
                    break;
            }
            try
            {
                await handler.OnHandle(this, header, payload);
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
            }
            return;
        }

        if (ConfigManager.Config.ServerOption.EnableDebug &&
                 ConfigManager.Config.ServerOption.DebugNoHandlerPacket)
            Logger.Error(I18NManager.Translate("Server.ConnectionInfo.NoHandlerFound",
                packetName ?? "opcode", opcode.ToString("X")));
    }

    private async Task SendDummy(string packetName)
    {
        var respName = packetName.Replace("Req", "Rsp"); // Get the response packet name
        if (respName == packetName) return; // do not send rsp when resp name = recv name
        var respOpcode = LogMap.FirstOrDefault(x => x.Value == respName).Key; // Get the response opcode

        // Send Rsp
        await SendPacket(respOpcode);
    }
    
    public async Task SetSecretKey(ulong seed)
    {
        var mt = new MT19937(seed);
        mt.Seed(mt.Int63());
        mt.Int63();

        await using var ms = new MemoryStream(0x1000);
        using var bw = new BinaryWriter(ms);
        for (int i = 0; i < 0x1000; i += 8)
        {
            bw.WriteUInt64BE(mt.Int63());
        }

        SecretKey = ms.ToArray();
    }
}
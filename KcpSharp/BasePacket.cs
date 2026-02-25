using NahidaImpact.Util.Extensions;
using Google.Protobuf;

namespace NahidaImpact.KcpSharp;

public class BasePacket(ushort cmdId)
{
    private const uint HEADER_CONST = 17767; // 0x4567
    private const uint TAIL_CONST = 0x89AB;
    public ushort CmdId { get; set; } = cmdId;
    private byte[] Header { get; set; } = [];
    public byte[] Payload { get; set; } = [];

    public void SetData(byte[] data)
    {
        Payload = data;
    }

    public void SetData(IMessage message)
    {
        Payload = message.ToByteArray();
    }

    public void SetData(string base64)
    {
        SetData(Convert.FromBase64String(base64));
    }

    public byte[] BuildPacket()
    {
        using MemoryStream? ms = new();
        using BinaryWriter? bw = new(ms);

        bw.WriteUInt16BE((ushort)HEADER_CONST);
        bw.WriteUInt16BE(CmdId);
        bw.WriteUInt16BE((ushort)(Header.Length));
        bw.WriteUInt32BE((uint)(Payload.Length));

        bw.Write(Header.ToArray());
        bw.Write(Payload.ToArray());

        bw.WriteUInt16BE((ushort)TAIL_CONST);

        var packet = ms.ToArray();

        return packet;
    }
}
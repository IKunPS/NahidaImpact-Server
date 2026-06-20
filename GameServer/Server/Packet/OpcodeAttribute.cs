namespace NahidaImpact.GameServer.Server.Packet;

[AttributeUsage(AttributeTargets.Class)]
public sealed class OpcodeAttribute(int cmdId) : Attribute
{
    public int CmdId { get; } = cmdId;
}
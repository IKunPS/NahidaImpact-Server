using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Inventory;

public class PacketWeaponAwakenRsp : BasePacket
{
    public PacketWeaponAwakenRsp(ulong targetGuid, Dictionary<uint, uint> oldAffixMap, Dictionary<uint, uint> curAffixMap, int awakenLevel, ulong avatarGuid = 0) : base(CmdIds.WeaponAwakenRsp)
    {
        var proto = new WeaponAwakenRsp
        {
            TargetWeaponGuid = targetGuid,
            TargetWeaponAwakenLevel = (uint)awakenLevel,
            AvatarGuid = avatarGuid,
            Retcode = 0
        };
        foreach (var kv in oldAffixMap)
            proto.OldAffixLevelMap[kv.Key] = kv.Value;
        foreach (var kv in curAffixMap)
            proto.CurAffixLevelMap[kv.Key] = kv.Value;
        SetData(proto);
    }
}
using NahidaImpact.Data;
using NahidaImpact.Database.Avatar;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Avatar;

public class PacketAvatarSkillDepotChangeNotify : BasePacket
{
    public PacketAvatarSkillDepotChangeNotify(AvatarDataInfo avatar)
        : base(CmdIds.AvatarSkillDepotChangeNotify)
    {
        var proto = new AvatarSkillDepotChangeNotify
        {
            AvatarGuid = avatar.Guid,
            SkillDepotId = avatar.SkillDepotId,
            CoreProudSkillLevel = avatar.CoreProudSkillLevel
        };

        foreach (var kv in avatar.SkillLevelMap)
            proto.SkillLevelMap[kv.Key] = kv.Value;
        foreach (var id in avatar.ProudSkillList)
            proto.ProudSkillList.Add(id);
        foreach (var id in avatar.TalentIdList)
            proto.TalentIdList.Add(id);
        foreach (var kv in avatar.ProudSkillExtraLevelMap)
            proto.ProudSkillExtraLevelMap[kv.Key] = kv.Value;

        SetData(proto);
    }
}
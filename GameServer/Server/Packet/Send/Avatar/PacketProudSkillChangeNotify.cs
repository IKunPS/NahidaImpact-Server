using NahidaImpact.Database.Avatar;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Avatar;

public class PacketProudSkillChangeNotify : BasePacket
{
    public PacketProudSkillChangeNotify(AvatarDataInfo avatar)
        : base(CmdIds.ProudSkillChangeNotify)
    {
        var proto = new ProudSkillChangeNotify
        {
            AvatarGuid = avatar.Guid,
            SkillDepotId = avatar.SkillDepotId
        };
        foreach (var id in avatar.ProudSkillList)
            proto.ProudSkillList.Add(id);
        SetData(proto);
    }
}
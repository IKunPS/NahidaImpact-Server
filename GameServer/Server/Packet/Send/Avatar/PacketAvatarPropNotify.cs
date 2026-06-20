using NahidaImpact.Database.Avatar;
using NahidaImpact.KcpSharp;
using NahidaImpact.Prop;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Avatar;

public class PacketAvatarPropNotify : BasePacket
{
    public PacketAvatarPropNotify(AvatarDataInfo avatar)
        : base(CmdIds.AvatarPropNotify)
    {
        var notify = new AvatarPropNotify { AvatarGuid = avatar.Guid };
        notify.PropMap[PlayerProp.PROP_LEVEL] = (long)avatar.Level;
        notify.PropMap[PlayerProp.PROP_EXP] = (long)avatar.Exp;
        notify.PropMap[PlayerProp.PROP_BREAK_LEVEL] = (long)avatar.PromoteLevel;
        SetData(notify);
    }
}

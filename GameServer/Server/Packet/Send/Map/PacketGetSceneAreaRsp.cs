using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Map;

public class PacketGetSceneAreaRsp : BasePacket
{
    public PacketGetSceneAreaRsp(PlayerInstance player, int sceneId) : base(CmdIds.GetSceneAreaRsp)
    {
        var proto = new GetSceneAreaRsp
        {
            SceneId = (uint)sceneId,
            Retcode = 0
        };

        // If no areas are unlocked for this scene, send all areas 1..999 as fallback
        var unlockedAreas = player.GetUnlockedSceneAreas(sceneId);
        if (unlockedAreas.Count == 0)
        {
            for (uint i = 1; i < 1000; i++)
                proto.AreaIdList.Add(i);
        }
        else
        {
            proto.AreaIdList.AddRange(unlockedAreas.Select(a => (uint)a));
        }

        // Add city info for cities 1-5 (Mondstadt, Liyue, Inazuma, Sumeru, Fontaine)
        // TODO: Use player.StatueOfTheSevenManager.GetCityInfo() when implemented
        for (uint i = 1; i <= 5; i++)
        {
            proto.CityInfoList.Add(new CityInfo { CityId = i, Level = 10, CrystalNum = 0 });
        }

        SetData(proto);
    }
}

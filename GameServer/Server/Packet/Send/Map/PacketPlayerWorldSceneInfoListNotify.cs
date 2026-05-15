using NahidaImpact.Data;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Map;

public class PacketPlayerWorldSceneInfoListNotify : BasePacket
{
    public PacketPlayerWorldSceneInfoListNotify(PlayerInstance player) : base(CmdIds.PlayerWorldSceneInfoListNotify)
    {
        var proto = new PlayerWorldSceneInfoListNotify();

        // Always add scene 1 as unlocked
        proto.InfoList.Add(new PlayerWorldSceneInfo
        {
            SceneId = 1,
            IsLocked = false
        });

        // Iterate over all scenes
        foreach (int scene in GameData.SceneData.Keys)
        {
            var worldInfoBuilder = new PlayerWorldSceneInfo
            {
                SceneId = (uint)scene,
                IsLocked = false
            };

            // Scene tags
            if (player.SceneTags.TryGetValue(scene, out var tags))
            {
                worldInfoBuilder.SceneTagIdList.AddRange(tags.Select(t => (uint)t));
            }

            // Map layer information for scene 3 (big world)
            if (scene == 3)
            {
                worldInfoBuilder.MapLayerInfo = new MapLayerInfo
                {
                    // These will be populated if MapLayer data is available
                };
                worldInfoBuilder.MapLayerInfo.UnlockedMapLayerIdList.AddRange(
                    GameData.MapLayerData.Keys.Select(k => (uint)k));
                worldInfoBuilder.MapLayerInfo.UnlockedMapLayerFloorIdList.AddRange(
                    GameData.MapLayerFloorData.Keys.Select(k => (uint)k));
                worldInfoBuilder.MapLayerInfo.UnlockedMapLayerGroupIdList.AddRange(
                    GameData.MapLayerGroupData.Keys.Select(k => (uint)k));
            }

            proto.InfoList.Add(worldInfoBuilder);
        }

        SetData(proto);
    }
}

using NahidaImpact.Data;
using NahidaImpact.Enums.Player;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.GameServer.Game.Worlds;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;
using NahidaImpact.Util.Extensions;

namespace NahidaImpact.GameServer.Server.Packet.Send.Player;

public class PacketPlayerEnterSceneNotify : BasePacket
{
    // Login
    public PacketPlayerEnterSceneNotify(PlayerInstance player) : base(CmdIds.PlayerEnterSceneNotify)
    {
        player.SceneLoadState = SceneLoadState.Loading;
        player.EnterToken = (uint)Extensions.RandomInt(1000, 99999);
        var now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        var uid = player.Uid;
        var sceneId = (int)player.SceneId;

        var proto = new PlayerEnterSceneNotify
        {
            SceneId = (uint)((sceneId - 49379) ^ 11523),
            Pos = player.Position.ToProto(),
            SceneBeginTime = ((ulong)now ^ 27843) + 16749,
            Type = EnterType.Self,
            TargetUid = (uint)((uid - 30259) ^ 4145),
            EnterSceneToken = (uint)((player.EnterToken ^ 57361) - 22665),
            WorldLevel = (uint)((player.Data.WorldLevel ^ 31579) + 19873),
            EnterReason = (uint)(((int)EnterReason.Login ^ 43962) + 40350),
            WorldType = (uint)((1 + 30022) ^ 64981),
            SceneTransaction = $"{sceneId}-{uid}-{(int)(now / 1000)}-18402",
            IsFirstLoginEnterScene = player.IsFirstLoginEnterScene
        };

        if (player.SceneId == 3)
        {
            proto.MapLayerInfo = new MapLayerInfo();
            proto.MapLayerInfo.UnlockedMapLayerIdList.AddRange(GameData.MapLayerData.Keys.Select(k => (uint)k));
            proto.MapLayerInfo.UnlockedMapLayerFloorIdList.AddRange(GameData.MapLayerFloorData.Keys.Select(k => (uint)k));
            proto.MapLayerInfo.UnlockedMapLayerGroupIdList.AddRange(GameData.MapLayerGroupData.Keys.Select(k => (uint)k));
        }
        if (player.SceneTags.TryGetValue(sceneId, out var tags))
            proto.SceneTagIdList.AddRange(tags.Select(t => (uint)t));

        SetData(proto);
    }

    // Teleport — all scene transitions route through TeleportProperties.
    public PacketPlayerEnterSceneNotify(PlayerInstance player, TeleportProperties props,
        int prevSceneId, Position prevPos, uint worldType = 1, int transactionId = 18402)
        : base(CmdIds.PlayerEnterSceneNotify)
    {
        player.SceneLoadState = SceneLoadState.Loading;
        player.EnterToken = (uint)Extensions.RandomInt(1000, 99999);
        var now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        var uid = player.Uid;

        var proto = new PlayerEnterSceneNotify
        {
            SceneId = (uint)((props.SceneId - 49379) ^ 11523),
            Pos = props.TeleportTo!.ToProto(),
            SceneBeginTime = ((ulong)now ^ 27843) + 16749,
            Type = props.EnterType,
            TargetUid = (uint)((uid - 30259) ^ 4145),
            EnterSceneToken = (uint)((player.EnterToken ^ 57361) - 22665),
            WorldLevel = (uint)((player.Data.WorldLevel ^ 31579) + 19873),
            EnterReason = (uint)(((int)props.EnterReason ^ 43962) + 40350),
            WorldType = worldType,
            SceneTransaction = $"{props.SceneId}-{uid}-{(int)(now / 1000)}-{transactionId}",
            PrevSceneId = (uint)((prevSceneId ^ 39512) - 40922),
            PrevPos = prevPos.ToProto()
        };

        if (props.DungeonId != 0)
            proto.DungeonId = (uint)((props.DungeonId ^ 27544) - 17829);

        SetData(proto);
    }
}

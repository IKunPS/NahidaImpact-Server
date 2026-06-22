using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.GameServer.Game.Worlds;
using NahidaImpact.GameServer.Server.Packet.Send.Map;
using NahidaImpact.GameServer.Server.Packet.Send.Scene;

namespace NahidaImpact.GameServer.Game.MapMarks;

public class MapMarksManager
{
    public const int MapMarkMaxCount = 150;

    private readonly PlayerInstance _player;

    public MapMarksManager(PlayerInstance player)
    {
        _player = player;
    }

    public Dictionary<string, MapMark> GetMapMarks() => _player.MapMarks;

    public void HandleMapMarkReq(MarkMapReq req)
    {
        switch (req.Op)
        {
            case MarkMapReq.Types.Operation.Add:
                if (req.Mark != null)
                {
                    var createMark = new MapMark(req.Mark);
                    // FishPool teleport: teleport player instead of marking
                    if (createMark.PointType == MapMarkPointType.FishPool)
                    {
                        Teleport(_player, createMark);
                        return;
                    }
                    AddMapMark(createMark);
                }
                break;

            case MarkMapReq.Types.Operation.Mod:
                if (req.Old != null && req.Old.Pos != null)
                    RemoveMapMark(new Position(
                        req.Old.Pos.X, req.Old.Pos.Y, req.Old.Pos.Z));
                if (req.Mark != null)
                {
                    var newMark = new MapMark(req.Mark);
                    AddMapMark(newMark);
                }
                break;

            case MarkMapReq.Types.Operation.Del:
                if (req.Mark != null && req.Mark.Pos != null)
                    RemoveMapMark(new Position(
                        req.Mark.Pos.X, req.Mark.Pos.Y, req.Mark.Pos.Z));
                break;
        }

        if (req.Op != MarkMapReq.Types.Operation.Get)
        {
            // TODO: Save to database
        }

        _ = _player.SendPacket(new PacketMarkMapRsp(GetMapMarks()));
    }

    public static string GetMapMarkKey(Position position)
    {
        return $"x{(int)position.X}z{(int)position.Z}";
    }

    public void RemoveMapMark(Position position)
    {
        GetMapMarks().Remove(GetMapMarkKey(position));
    }

    public void AddMapMark(MapMark mapMark)
    {
        if (GetMapMarks().Count < MapMarkMaxCount && mapMark.Position != null)
        {
            GetMapMarks()[GetMapMarkKey(mapMark.Position)] = mapMark;
        }
    }

    /// <summary>
    /// Teleport player to a FishPool mark. Mirrors Java MapMarksManager.teleport().
    /// </summary>
    private void Teleport(PlayerInstance player, MapMark mapMark)
    {
        float y;
        try
        {
            y = float.Parse(mapMark.Name);
        }
        catch
        {
            y = 300;
        }

        var pos = mapMark.Position;
        if (pos == null) return;

        player.World.TransferPlayerToScene(
            player,
            mapMark.SceneId,
            new Position(pos.X, y, pos.Z));

        player.Scene?.BroadcastPacket(new PacketSceneEntityAppearNotify(player));
    }
}

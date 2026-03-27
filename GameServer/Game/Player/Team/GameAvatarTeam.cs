using System.Collections.Generic;

namespace NahidaImpact.GameServer.Game.Player.Team;

public class GameAvatarTeam
{
    public uint Index { get; set; }
    public string Name { get; set; } = "";
    public List<ulong> AvatarGuidList { get; set; }
    
    public int Size => AvatarGuidList.Count;

    public GameAvatarTeam()
    {
        AvatarGuidList = new();
    }

    public GameAvatarTeam(List<ulong> avatarGuidList)
    {
        AvatarGuidList = avatarGuidList;
    }

    public bool Contains(ulong avatarGuid) => AvatarGuidList.Contains(avatarGuid);

    public bool AddAvatar(ulong avatarGuid)
    {
        if (Contains(avatarGuid)) return false;
        AvatarGuidList.Add(avatarGuid);
        return true;
    }

    public bool RemoveAvatar(int slot)
    {
        if (Size <= 1) return false;
        AvatarGuidList.RemoveAt(slot);
        return true;
    }

    public void CopyFrom(GameAvatarTeam team, int maxTeamSize)
    {
        var avatarGuids = new List<ulong>(team.AvatarGuidList);
        AvatarGuidList.Clear();

        int len = System.Math.Min(avatarGuids.Count, maxTeamSize);
        for (int i = 0; i < len; i++)
        {
            AvatarGuidList.Add(avatarGuids[i]);
        }
    }
}

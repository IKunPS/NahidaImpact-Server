using System.Collections;
using System.Collections.Generic;
using NahidaImpact.Database.Avatar;
using NahidaImpact.GameServer.Game.Player;

namespace NahidaImpact.GameServer.Game.Avatar;

public class AvatarStorage : BasePlayerManager, IEnumerable<AvatarDataInfo>
{
    public PlayerInstance Player { get; set; }
    
    private readonly Dictionary<uint, AvatarDataInfo> avatars = new();
    private readonly Dictionary<ulong, AvatarDataInfo> avatarsGuid = new();

    public AvatarStorage(PlayerInstance player) : base(player)
    {
        Player = player;
    }

    public int AvatarCount => this.avatars.Count;
    public AvatarDataInfo? GetAvatarById(uint id) =>
        avatars.TryGetValue(id, out var avatar) ? avatar : null;

    public AvatarDataInfo? GetAvatarByGuid(ulong id) =>
        avatarsGuid.TryGetValue(id, out var avatar) ? avatar : null;
    public bool HasAvatar(uint id) => this.avatars.ContainsKey(id);

    public bool AddAvatar(AvatarDataInfo avatar)
    {
        if (avatar == null || HasAvatar(avatar.AvatarId))
            return false;
        
        this.avatars[avatar.AvatarId] = avatar;
        this.avatarsGuid[avatar.Guid] = avatar;

        avatar.Save();
        return true;
    }

    public IEnumerator<AvatarDataInfo> GetEnumerator()
    {
        return this.avatars.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
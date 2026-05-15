using NahidaImpact.Data;
using NahidaImpact.Data.Binout;
using NahidaImpact.Data.Excel;
using NahidaImpact.Enums.Item;
using NahidaImpact.Enums.Player;
using NahidaImpact.GameServer.Server.Packet.Send.Map;
using NahidaImpact.GameServer.Server.Packet.Send.State;
using NahidaImpact.Util;

namespace NahidaImpact.GameServer.Game.Player;

public class ProgressManager(PlayerInstance player) : BasePlayerManager(player)
{
    public Dictionary<int, int> OpenStates = new();
    public static readonly HashSet<int> BlacklistOpenStates = new() { 48 }; // OPEN_STATE_LIMIT_REGION_GLOBAL
    public static readonly HashSet<int> IgnoredOpenStates = new() { 1404 }; // OPEN_STATE_MENGDE_INFUSEDCRYSTAL
    public static HashSet<int> DefaultOpenStates = CalculateDefaultOpenStates();
    
    public void OnPlayerLogin()
    {
        // Try unlocking open states
        TryUnlockOpenStates(false);

        // Add statue quests if necessary
        AddStatueQuestsOnLogin();

        // Determine whether to unlock all map points
        if (ConfigManager.Config.GameOptions.DefaultUnlockAllMap)
        {
            // Unlock all map points for scenes 3-11 and areas 1-999
            for (int i = 3; i <= 11; i++)
            {
                var sceneAreas = Enumerable.Range(1, 999).ToHashSet();
                player.GetUnlockedSceneAreas(i).UnionWith(sceneAreas);
            }

            foreach (var (sceneId, scenePoints) in GameData.ScenePointsPerScene)
            {
                player.GetUnlockedScenePoints(sceneId).UnionWith(scenePoints);
            }
        }
        else
        {
            // Unlock starting teleport point (7) and Starfell Lake area (1) in scene 3 only
            player.GetUnlockedScenePoints(3).Add(7);
            player.GetUnlockedSceneAreas(3).Add(1);
        }

        // Allow the player to visit all areas (open state 47), removes Paimon barrier
        SetOpenState(47, 1, true);
        // Disable world boundary red barrier (OPEN_STATE_LIMIT_REGION_GLOBAL = 48)
        // This state is blacklisted from auto-unlock and must be explicitly set
        SetOpenState(48, 1, true);
    }

    public bool UnlockTransPoint(int sceneId, int pointId, bool isStatue)
    {
        // Check whether the unlocked point exists
        var entry = GameData.GetScenePointEntryById(sceneId, pointId);
        if (entry == null)
        {
            return false;
        }

        // Check if already unlocked
        if (player.GetUnlockedScenePoints(sceneId).Contains(pointId))
        {
            return false;
        }

        // Add to unlocked set
        player.GetUnlockedScenePoints(sceneId).Add(pointId);

        // Give rewards: 5 primogems + adventure EXP
        _ = player.InventoryManager.AddItem(201, 5); // Primogems
        _ = player.InventoryManager.AddItem(102, isStatue ? 50 : 10); // Adventure EXP

        // Fire quest trigger for trans point unlock
        // TODO: player.QuestManager?.QueueEvent(QuestContent.QUEST_CONTENT_UNLOCK_TRANS_POINT, sceneId, pointId);
        // TODO: player.Scene?.ScriptManager?.CallEvent(new ScriptArgs(EVENT_UNLOCK_TRANS_POINT, sceneId, pointId));

        // Send notification
        _ = player.SendPacket(new PacketScenePointUnlockNotify(sceneId, pointId));
        return true;
    }

    public void UnlockSceneArea(int sceneId, int areaId)
    {
        var areas = player.GetUnlockedSceneAreas(sceneId);
        if (areas.Contains(areaId)) return;

        areas.Add(areaId);
        _ = player.SendPacket(new PacketSceneAreaUnlockNotify(sceneId, areaId));
    }

    public void UnlockSceneArea(int sceneId, IEnumerable<int> areaIds)
    {
        var areas = player.GetUnlockedSceneAreas(sceneId);
        var newAreas = areaIds.Where(a => !areas.Contains(a)).ToList();
        if (newAreas.Count == 0) return;

        foreach (var a in newAreas)
            areas.Add(a);
        _ = player.SendPacket(new PacketSceneAreaUnlockNotify(sceneId, newAreas));
    }
    
    private static HashSet<int> CalculateDefaultOpenStates()
    {
        var result = new HashSet<int>();
        foreach (var state in GameData.OpenStateData.Values)
        {
            var cond = state.Cond ?? new List<OpenStateCond>();
            
            // Check if state meets any of the default open state conditions
            if (state.DefaultState && !state.AllowClientOpen // Actual default-opened states.
                || (cond.Count == 1
                    && cond[0].CondType == OpenStateCondType.OPEN_STATE_COND_PLAYER_LEVEL
                    && cond[0].Param == 1)
                || cond.Any(c => c.CondType == OpenStateCondType.OPEN_STATE_OFFERING_LEVEL
                                 || c.CondType == OpenStateCondType.OPEN_STATE_CITY_REPUTATION_LEVEL)
                || state.Id == 1) // Always unlock OPEN_STATE_PAIMON, otherwise the player will not have a working chat.
            {
                result.Add((int)state.Id);
            }
        }

        return result;
    }
    
    public int GetOpenState(int openState)
    {
        return OpenStates.TryGetValue(openState, out int value) ? value : 0;
    }
    
    private void SetOpenState(int openState, int value, bool sendNotify)
    {
        int previousValue = OpenStates.TryGetValue(openState, out int temp) ? temp : -1; // -1 indicates non-existent

        if (value != previousValue)
        {
            OpenStates[openState] = value;

            // TODO: Trigger quest event QUEST_COND_OPEN_STATE_EQUAL
            // player.QuestManager?.QueueEvent(QuestCond.QUEST_COND_OPEN_STATE_EQUAL, openState, value);

            if (sendNotify)
            {
                _ = player.SendPacket(new PacketOpenStateChangeNotify(openState, value));
            }
        }
    }
    
    private void SetOpenState(int openState, int value)
    {
        SetOpenState(openState, value, true);
    }
    
    public void SetOpenStateFromClient(int openState, int value)
    {
        // TODO: Implement client-openable state validation
        // For now, just set
        SetOpenState(openState, value);
    }
    
    public void ForceSetOpenState(int openState, int value)
    {
        SetOpenState(openState, value);
    }
    
    public void TryUnlockOpenStates(bool sendNotify)
    {
        // Get list of open states that are not yet unlocked.
        var lockedStates = GameData.OpenStateData.Values
            .Where(s => GetOpenState((int)s.Id) == 0)
            .ToList();
        
        foreach (var state in lockedStates)
        {
            // To auto-unlock a state, it has to meet three conditions:
            // * it can not be a state that is unlocked by the client,
            // * it has to meet all its unlock conditions, and
            // * it can not be in the blacklist.
            // TODO: Implement condition checking
            // For now, we only auto-unlock default states that are not client-openable
            if (!state.AllowClientOpen && 
                !BlacklistOpenStates.Contains((int)state.Id) &&
                !IgnoredOpenStates.Contains((int)state.Id))
            {
                SetOpenState((int)state.Id, 1, sendNotify);
            }
        }
    }
    
    public void TryUnlockOpenStates()
    {
        TryUnlockOpenStates(true);
    }
    
    private void AddStatueQuestsOnLogin()
    {
        // TODO: Implement quest system integration
        // Get all currently existing subquests for the "unlock all statues" main quest (303).
        // var statueMainQuest = GameData.GetMainQuestDataMap().get(303);
        // if (statueMainQuest != null)
        // {
        //     var statueGameMainQuest = player.QuestManager?.GetMainQuestById(303);
        //     if (statueGameMainQuest == null)
        //     {
        //         player.QuestManager?.AddQuest(30302);
        //     }
        //     foreach (var subData in statueMainQuest.GetSubQuests())
        //     {
        //         var subGameQuest = statueGameMainQuest?.GetChildQuestById(subData.SubId);
        //         if (subGameQuest?.State == QuestState.QUEST_STATE_UNSTARTED)
        //         {
        //             player.QuestManager?.AddQuest(subData.SubId);
        //         }
        //     }
        // }
    }
    
    public void AddSceneTag(int sceneId, int sceneTagId)
    {
        if (!player.SceneTags.TryGetValue(sceneId, out var tags))
        {
            tags = [];
            player.SceneTags[sceneId] = tags;
        }
        tags.Add(sceneTagId);
        _ = player.SendPacket(new PacketPlayerWorldSceneInfoListNotify(player));
    }
    
    public void DelSceneTag(int sceneId, int sceneTagId)
    {
        if (player.SceneTags.TryGetValue(sceneId, out var tags))
        {
            tags.Remove(sceneTagId);
            _ = player.SendPacket(new PacketPlayerWorldSceneInfoListNotify(player));
        }
    }
    
    public bool CheckSceneTag(int sceneId, int sceneTagId)
    {
        return player.SceneTags.TryGetValue(sceneId, out var tags) && tags.Contains(sceneTagId);
    }
}
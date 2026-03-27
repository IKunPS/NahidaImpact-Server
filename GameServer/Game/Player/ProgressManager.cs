using NahidaImpact.Data;
using NahidaImpact.Data.Excel;
using NahidaImpact.Enums.Player;

namespace NahidaImpact.GameServer.Game.Player;

public class ProgressManager
{
    private PlayerInstance Owner;
    
    public Dictionary<int, int> OpenStates = new();
    
    public static readonly HashSet<int> BlacklistOpenStates = new() { 48 }; // OPEN_STATE_LIMIT_REGION_GLOBAL
    
    public static readonly HashSet<int> IgnoredOpenStates = new() { 1404 }; // OPEN_STATE_MENGDE_INFUSEDCRYSTAL
    
    public static HashSet<int> DefaultOpenStates = CalculateDefaultOpenStates();
    
    public ProgressManager(PlayerInstance player)
    {
        Owner = player;
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
            
            if (sendNotify)
            {
                // TODO: Send PacketOpenStateChangeNotify
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
}
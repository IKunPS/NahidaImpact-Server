namespace NahidaImpact.Data.Common;

/// <summary>
/// Custom trial avatar data loaded from CustomResources/TrialAvatarExcels/TrialAvatarData.json.
/// Mirrors Java TrialAvatarCustomData.
/// </summary>
public class TrialAvatarCustomData
{
    public int TrialAvatarId { get; set; }
    public List<string> TrialAvatarParamList { get; set; } = [];
    public int CoreProudSkillLevel { get; set; }
    public int SkillDepotId { get; set; }
}

using System.Text.Json.Serialization;

namespace NahidaImpact.Data.Excel;

/// <summary>
/// Trial avatar config loaded from ExcelBinOutput/TrialAvatarExcelConfigData.json.
/// Mirrors Java TrialAvatarData.
/// </summary>
[ResourceEntity("TrialAvatarExcelConfigData.json")]
public class TrialAvatarDataExcel : ExcelResource
{
    [JsonPropertyName("trialAvatarId")]
    public uint TrialAvatarId { get; set; }

    [JsonPropertyName("trialAvatarParamList")]
    public List<int> TrialAvatarParamList { get; set; } = [];

    public override uint GetId() => TrialAvatarId;

    public override void Loaded()
    {
        GameData.TrialAvatarDataMap[(int)TrialAvatarId] = this;
    }
}

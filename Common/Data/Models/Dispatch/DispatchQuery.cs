using System.ComponentModel.DataAnnotations;

namespace NahidaImpact.Data.Models.Dispatch;

public class DispatchQuery
{
    [Required] public string? Version { get; set; }
    public string? Key_Id { get; set; }
    public string? DispatchSeed { get; set; }
}
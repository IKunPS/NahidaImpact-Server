using Microsoft.AspNetCore.Mvc;

namespace NahidaImpact.Models.Dispatch;

public class DispatchQuery
{
    [FromQuery(Name = "version")]
    public string? Version { get; set; }

    [FromQuery(Name = "key_id")]
    public string? Key_Id { get; set; }

    [FromQuery(Name = "dispatchSeed")]
    public string? DispatchSeed { get; set; }
}

using NahidaImpact.Data.Models.Sdk;
using Microsoft.AspNetCore.Mvc;

namespace NahidaImpact.SdkServer.Handlers.Sdk;

[ApiController]
public class RiskyController : ControllerBase
{
    [HttpPost("/account/risky/api/check")]
    public IActionResult ComboGranter()
    {
        return Ok(new ResponseBase
        {
            Data = new
            {
                id = "none",
                action = "ACTION_NONE",
                geetest = "null"
            }
        });
    }
}
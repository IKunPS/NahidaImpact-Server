using NahidaImpact.Util;
using Microsoft.AspNetCore.Mvc;

namespace NahidaImpact.SdkServer.Handlers.Sdk;

[ApiController]
public class ServerStatusController : ControllerBase
{
    [HttpGet("/status/server")]
    public IActionResult GetServerStatus()
    {
        try
        {
            return Ok(new
            {
                retcode = 0,
                status = new
                {
                    version = GameConstants.GAME_VERSION
                }
            });
        }
        catch (Exception e)
        {
            return Ok(new { retcode = -1, message = e.Message });
        }
    }
}
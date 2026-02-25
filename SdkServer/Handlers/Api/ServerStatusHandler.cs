using NahidaImpact.Util;
using Microsoft.AspNetCore.Mvc;

namespace NahidaImpact.SdkServer.Handlers.Api;

[ApiController]
public class ServerStatusHandler : ControllerBase
{
    [HttpGet("/status/server")]
    public IActionResult GetServerStatus()
    {
        try
        {
            var response = new
            {
                retcode = 0,
                status = new
                {
                    version = GameConstants.GAME_VERSION
                }
            };

            return Ok(response);
                
        }
        catch (Exception e)
        {
            return Ok(new { retcode = -1, message = e.Message });
        }
    }
}
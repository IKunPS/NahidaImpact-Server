using NahidaImpact.Data.Models.Sdk;
using Microsoft.AspNetCore.Mvc;

namespace NahidaImpact.SdkServer.Handlers.Sdk;

[ApiController]
public class AbTestController : ControllerBase
{
    [HttpPost("/data_abtest_api/config/experiment/list")]
    public IActionResult GetExperimentList()
    {
        return Ok(new ResponseBase
        {
            Data = new[]
            {
                new
                {
                    code = 1000,
                    type = 2,
                    config_id = "14",
                    period_id = "6036_99",
                    version = 1,
                    configs = new
                    {
                        hoyopass_enable = false,
                        cardType = "old"
                    },
                    sceneWhiteList = false,
                    experimentWhiteList = false,
                }
            }
        });
    }
}
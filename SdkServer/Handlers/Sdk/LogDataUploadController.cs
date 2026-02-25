using Microsoft.AspNetCore.Mvc;

namespace NahidaImpact.SdkServer.Handlers.Sdk;

[ApiController]
public class LogDataUploadController : ControllerBase
{
    [HttpGet("/perf/config/verify")]
    public IActionResult GetVerify()
    {
        return Ok(new { code = 0, message = "OK" });
    }

    [HttpPost("/{logType}/sdk/upload")]
    public IActionResult LogSdkUpload()
    {
        return Ok(new { code = 0, message = "OK" });
    }

    [HttpPost("/common/h5log/log/batch")]
    public IActionResult H5LogBatch()
    {
        return Ok(new { code = 0, message = "OK" });
    }

    [HttpGet("/sdk/upload")]
    public IActionResult GetSdkUpload()
    {
        return Ok(new { code = 0, message = "OK" });
    }

    [HttpPost("/sdk/dataUpload")]
    public IActionResult DataUpload()
    {
        return Ok(new { code = 0, message = "OK" });
    }
}
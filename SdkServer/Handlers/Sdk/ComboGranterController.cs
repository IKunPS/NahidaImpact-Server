using Microsoft.AspNetCore.Mvc;
using NahidaImpact.Database.Account;
using NahidaImpact.Internationalization;
using NahidaImpact.Models.Sdk;
using NahidaImpact.Util;

namespace NahidaImpact.SdkServer.Handlers.Sdk;

[ApiController]
public class ComboGranterController : Controller
{
    [HttpPost("/{productName}/combo/granter/login/v2/login")]
    public async Task<IActionResult> ComboLoginV2(string productName, [FromBody] ComboGranterRequest request)
    {
        if (request.Data?.Uid == null || !int.TryParse(request.Data.Uid, out var accountUid))
        {
            return Ok(new ResponseBase
            {
                Retcode = -101,
                Success = false,
                Message = I18NManager.Translate("Server.Web.TokenError")
            });
        }

        var account = AccountData.GetAccountByUid(accountUid, true);

        if (account == null || account.ComboToken != request.Data?.Token)
        {
            return Ok(new ResponseBase
            {
                Retcode = -101,
                Success = false,
                Message = I18NManager.Translate("Server.Web.TokenError")
            });
        }

        return Ok(new ComboGranterResponse
        {
            Data = new ComboGranterResponse.ComboGranterResponseData
            {
                AccountType = 1,
                Data = "{\"guest\": false}",
                Heartbeat = false,
                OpenId = account.Uid.ToString(),
                ComboToken = account.ComboToken,
            },
        });
    }

    [HttpPost("/{productName}/combo/granter/api/compareProtocolVersion")] [HttpGet("/{productName}/combo/granter/api/compareProtocolVersion")]
    public IActionResult CompareProtocolVersion(string productName)
    {
        return Ok(new ResponseBase
        {
            Data = new
            {
                Modified = true,
                protocol = new
                {
                    id = 0,
                    app_id = 4,
                    language = "en",
                    major = 0,
                    minimum = 0,
                    create_time = 0
                }
            }
        });
    }

    [HttpGet("/{productName}/combo/granter/api/getConfig")]
    public IActionResult GetConfig()
    {
        return Ok(new ResponseBase
        {
            Data = new
            {
                protocol = true,
                qr_enabled = false,
                log_level = "INFO",
                announce_url =
                    $"{ConfigManager.Config.HttpServer.GetDisplayAddress()}/hk4e/announcement/index.html",
                push_alias_type = 2,
                disable_ysdk_guard = false,
                enable_announce_popup = false,
                enable_announce_pic_popup = true,
            }
        });
    }

    [HttpGet("/combo/box/api/config/sdk/combo")]
    public IActionResult GetComboConfig()
    {
        return Ok(new ResponseBase
        {
            Data = new
            {
                vals = new
                {
                    disable_email_bind_skip = false,
                    email_bind_remind_interval = 7,
                    email_bind_remind = true
                }
            }
        });
    }
}
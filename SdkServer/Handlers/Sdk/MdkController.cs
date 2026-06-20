using Microsoft.AspNetCore.Mvc;
using NahidaImpact.Database.Account;
using NahidaImpact.Internationalization;
using NahidaImpact.Models.Sdk;
using NahidaImpact.Util;

namespace NahidaImpact.SdkServer.Handlers.Sdk;

[ApiController]
public class MdkController : Controller
{
    [HttpPost("/{productName}/mdk/shield/api/login")]
    public async Task<IActionResult> MdkShieldLogin(string productName, [FromBody] MdkShieldLoginRequest request)
    {
        var account = AccountData.GetAccountByUserName(request.Account ?? string.Empty);

        if (account == null && !ConfigManager.Config.ServerOption.AutoCreateUser)
        {
            return Ok(new ResponseBase
            {
                Retcode = -101,
                Success = false,
                Message = I18NManager.Translate("Server.Web.AccountNotFound")
            });
        }

        if (account == null && ConfigManager.Config.ServerOption.AutoCreateUser)
        {
            AccountData.CreateAccount(request.Account ?? string.Empty, 0, request.Password ?? string.Empty);
            account = AccountData.GetAccountByUserName(request.Account ?? string.Empty);
        }

        if (account == null)
        {
            return Ok(new ResponseBase
            {
                Retcode = -101,
                Success = false,
                Message = I18NManager.Translate("Server.Web.AccountNotFound")
            });
        }

        return Ok(new MdkShieldResponse
        {
            Data = new MdkShieldResponse.MdkShieldResponseData
            {
                Account = new MdkShieldAccountData
                {
                    Uid = account.Uid.ToString(),
                    Token = account.GenerateComboToken(),
                    Name = account.Username,
                    Realname = account.Username,
                    IsEmailVerify = "0",
                    Email = $"{account.Username}@neonteam.dev",
                    AreaCode = "**",
                    Country = "US",
                },
            }
        });
    }

    [HttpPost("/{productName}/mdk/shield/api/verify")]
    public async Task<IActionResult> MdkShieldVerify(string productName, [FromBody] MdkShieldVerifyRequest request)
    {
        if (request.Uid == null || !int.TryParse(request.Uid, out var accountUid))
        {
            return Ok(new ResponseBase
            {
                Retcode = -101,
                Success = false,
                Message = I18NManager.Translate("Server.Web.CacheError")
            });
        }

        var account = AccountData.GetAccountByUid(accountUid, true);

        if (account == null)
        {
            return Ok(new ResponseBase
            {
                Retcode = -101,
                Success = false,
                Message = I18NManager.Translate("Server.Web.CacheError")
            });
        }

        if (account.ComboToken != request.Token)
        {
            return Ok(new ResponseBase
            {
                Retcode = -101,
                Success = false,
                Message = I18NManager.Translate("Server.Web.Relogin")
            });
        }

        return Ok(new MdkShieldResponse
        {
            Data = new MdkShieldResponse.MdkShieldResponseData
            {
                Account = new MdkShieldAccountData
                {
                    Uid = account.Uid.ToString(),
                    Token = account.ComboToken!,
                    Name = account.Username,
                    Realname = account.Username,
                    IsEmailVerify = "0",
                    Email = $"{account.Username}@neonteam.dev",
                    AreaCode = "**",
                    Country = "US",
                },
            }
        });
    }

    [HttpGet("/{productName}/mdk/agreement/api/getAgreementInfos")]
    public IActionResult MdkGetAgreementInfos(string productName)
    {
        return Ok(new ResponseBase
        {
            Data = new { marketing_agreements = Array.Empty<object>() }
        });
    }

    [HttpGet("/{productName}/mdk/shield/api/loadConfig")]
    public IActionResult MdkLoadConfig(string productName)
    {
        return Ok(new ResponseBase
        {
            Data = new
            {
                id = 6,
                game_key = productName,
                client = "PC",
                identity = "I_IDENTITY",
                guest = false,
                scene = "S_NORMAL",
                name = "原神海外",
                disable_regist = false,
                enable_email_captcha = false,
                thirdparty = "[\"fb\",\"tw\"]",
                disable_mmt = false,
                server_guest = true,
                thirdparty_ignore = "{\"tw\":\"\",\"fb\":\"\"}",
                enable_ps_bind_account = false,
                thirdparty_login_configs = new
                {
                    tw = new { token_type = "TK_GAME_TOKEN", game_token_expires_in = "604800" },
                    fb = new { token_type = "TK_GAME_TOKEN", game_token_expires_in = "604800" }
                }
            }
        });
    }
}
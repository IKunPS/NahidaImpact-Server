using NahidaImpact.Data.Models.Sdk;
using NahidaImpact.Database.Account;
using NahidaImpact.Util;
using Microsoft.AspNetCore.Mvc;

namespace NahidaImpact.SdkServer.Handlers.Sdk;

[ApiController]
public class MdkController : Controller
{
    [HttpPost("/{productName}/mdk/shield/api/login")]
    public async Task<IActionResult> MdkShieldLogin(string productName, [FromBody] MdkShieldLoginRequest request)
    {
        var account = AccountData.GetAccountByUserName(request.Account!);

        if (account == null && !ConfigManager.Config.ServerOption.AutoCreateUser) 
        {
            return Ok(new ResponseBase
            {
                Retcode = -101,
                Success = false,
                Message = "Account not found"
            });
        };

        // Make new account
        if (account == null && ConfigManager.Config.ServerOption.AutoCreateUser)
        {
            AccountData.CreateAccount(request.Account!, 0, request.Password!);

            account = AccountData.GetAccountByUserName(request.Account!)!;
        };

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
                    Email = $"{account!.Username}@neonteam.dev",
                    AreaCode = "**",
                    Country = "US",
                },
            }
        });
    }

    [HttpPost("/{productName}/mdk/shield/api/verify")]
    public async Task<IActionResult> MdkShieldVerify(string productName, [FromBody] MdkShieldVerifyRequest request)
    {
        int accountUid;
        try
        {
            accountUid = int.Parse(request.Uid!);
        }
        catch
        {
            return Ok(new ResponseBase
            {
                Retcode = -101,
                Success = false,
                Message = "Account cache error"
            });
        }

        var account = AccountData.GetAccountByUid(accountUid,true);

        if (account == null)
        {
            return Ok(new ResponseBase
            {
                Retcode = -101,
                Success = false,
                Message = "Account cache error"
            });
        }

        if (account.ComboToken != request.Token)
        {
            return Ok(new ResponseBase
            {
                Retcode = -101,
                Success = false,
                Message = "For account safety, please log in again"
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
                    Email = $"{account!.Username}@neonteam.dev",
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
                    tw = new
                    {
                        token_type = "TK_GAME_TOKEN",
                        game_token_expires_in = "604800"
                    },
                    fb = new
                    {
                        token_type = "TK_GAME_TOKEN",
                        game_token_expires_in = "604800"
                    }
                }
            }
        });
    }
}
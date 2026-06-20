using Microsoft.AspNetCore.Mvc;
using NahidaImpact.Database.Account;
using NahidaImpact.Internationalization;
using NahidaImpact.Models.Sdk;
using NahidaImpact.Util;
using NahidaImpact.Util.Security;
using System.Security.Cryptography;
using System.Text;

namespace NahidaImpact.SdkServer.Handlers.Sdk;

[ApiController]
public class MaPassportController : ControllerBase
{
    private static readonly Logger Logger = new("MaPassportController");

    [HttpPost("/{productName}/account/ma-passport/api/appLoginByPassword")]
    public async Task<IActionResult> AppLoginByPassword(string productName,
        [FromBody] AppLoginByPasswordRequest request)
    {
        string? account = request.Account;
        string? password = request.Password;

        if (!string.IsNullOrEmpty(account) && account.Length > 32)
        {
            try
            {
                if (Crypto.SDK_PATCH_KEY == null)
                {
                    return Ok(new ResponseBase
                    {
                        Retcode = -201,
                        Message = I18NManager.Translate("Server.Web.LoginFailed")
                    });
                }

                byte[] encryptedAccountBytes = Convert.FromBase64String(account);
                byte[] decryptedAccountBytes = Crypto.SDK_PATCH_KEY.Decrypt(
                    encryptedAccountBytes, RSAEncryptionPadding.Pkcs1);
                account = Encoding.UTF8.GetString(decryptedAccountBytes);

                Logger.Info($"clientLogin account: {account}");

                if (!string.IsNullOrEmpty(password))
                {
                    byte[] encryptedPasswordBytes = Convert.FromBase64String(password);
                    byte[] decryptedPasswordBytes = Crypto.SDK_PATCH_KEY.Decrypt(
                        encryptedPasswordBytes, RSAEncryptionPadding.Pkcs1);
                    password = Encoding.UTF8.GetString(decryptedPasswordBytes);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"clientLogin decryption error: {ex.Message}");
                return Ok(new ResponseBase
                {
                    Retcode = -201,
                    Message = I18NManager.Translate("Server.Web.DecryptionFailed")
                });
            }
        }

        var accountData = AccountData.FindAccountByUsername(account);

        if (accountData == null && ConfigManager.Config.ServerOption.AutoCreateUser)
        {
            var (success, accountUid) = await AccountData.CreateAccount(account, password);
            if (!success)
            {
                return Ok(new ResponseBase
                {
                    Retcode = -101,
                    Message = I18NManager.Translate("Server.Web.CreateAccountFailed")
                });
            }
            accountData = AccountData.FindAccountByAccountUid(accountUid);
        }

        if (accountData == null)
        {
            return Ok(new ResponseBase
            {
                Retcode = -101,
                Message = I18NManager.Translate("Server.Web.AccountNotFound")
            });
        }

        if (!AccountData.VerifyPassword(accountData, password))
        {
            return Ok(new ResponseBase
            {
                Retcode = -201,
                Message = I18NManager.Translate("Server.Web.PasswordIncorrect")
            });
        }

        return Ok(new AppLoginByPasswordResponse
        {
            Retcode = 0,
            Message = I18NManager.Translate("Server.Web.OK"),
            Data = new AppLoginByPasswordResponse.AppLoginByPasswordAccountData
            {
                Token = new AppLoginByPasswordToken
                {
                    Token = accountData.GenerateComboToken()
                },
                UserInfo = new AppLoginByPasswordUserInfo
                {
                    aid = accountData.Uid.ToString(),
                    mid = accountData.Uid.ToString(),
                    AccountName = accountData.Username,
                    Email = $"{accountData.Username}@neonteam.dev",
                    IsEmailVerify = 0
                }
            }
        });
    }

    [HttpPost("/{productName}/account/ma-passport/token/verifySToken")]
    public IActionResult Logout(string productName, [FromBody] AppLoginByPasswordRequest request)
    {
        return Ok(new AppLoginByPasswordResponse
        {
            Data = new AppLoginByPasswordResponse.AppLoginByPasswordAccountData
            {
                Token = new AppLoginByPasswordToken()
            }
        });
    }
}
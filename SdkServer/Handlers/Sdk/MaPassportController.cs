using Microsoft.AspNetCore.Mvc;
using NahidaImpact.Data.Models.Sdk;
using NahidaImpact.Util;
using NahidaImpact.Util.Security;
using System.Security.Cryptography;
using System.Text;
using NahidaImpact.Database.Account;

namespace NahidaImpact.SdkServer.Handlers.Sdk;

[ApiController]
public class MaPassportController : ControllerBase
{
    [HttpPost("/{productName}/account/ma-passport/api/appLoginByPassword")]
    public async Task<IActionResult> AppLoginByPassword(string productName,
        [FromBody] AppLoginByPasswordRequest response)
    {
        string? account = response.Account;
        string? password = response.Password;

        // 如果 account 长度大于 32，说明是加密的，需要解密
        // If the account length is greater than 32, it means it is encrypted and needs to be decrypted
        if (!string.IsNullOrEmpty(account) && account.Length > 32)
        {
            try
            {
                if (Crypto.SDK_PATCH_KEY == null)
                {
                    return Ok(new ResponseBase
                    {
                        Retcode = -201,
                        Message = "Account login failed, signature key not initialized"
                    });
                }

                // 解密 account
                // Decrypt account
                byte[] encryptedAccountBytes = Convert.FromBase64String(account);
                byte[] decryptedAccountBytes = Crypto.SDK_PATCH_KEY.Decrypt(
                    encryptedAccountBytes, RSAEncryptionPadding.Pkcs1);
                account = Encoding.UTF8.GetString(decryptedAccountBytes);

                Logger logger = new("MaPassportController");
                logger.Info($"clientLogin account: {account}");

                // 解密 password
                // Decrypt password
                if (!string.IsNullOrEmpty(password))
                {
                    byte[] encryptedPasswordBytes = Convert.FromBase64String(password);
                    byte[] decryptedPasswordBytes = Crypto.SDK_PATCH_KEY.Decrypt(
                        encryptedPasswordBytes, RSAEncryptionPadding.Pkcs1);
                    password = Encoding.UTF8.GetString(decryptedPasswordBytes);

                    logger.Info($"clientLogin password: {password}");
                }
            }
            catch (Exception e)
            {
                Logger logger = new("MaPassportController");
                logger.Error($"clientLogin error: {e}");

                return Ok(new ResponseBase
                {
                    Retcode = -201,
                    Message = "Account login failed, account name decryption failed"
                });
            }
        }
        
        var accountData = AccountRepository.FindAccountByUsername(account);
        
        if (accountData == null && ConfigManager.Config.ServerOption.AutoCreateUser)
        {
            var (success, accountUid) = await AccountRepository.CreateAccount(account, password);
            if (!success)
            {
                return Ok(new ResponseBase
                {
                    Retcode = -101,
                    Message = "Failed to create account"
                });
            }

            accountData = AccountRepository.FindAccountByAccountUid(accountUid);
        }
        
        if (accountData == null)
        {
            return Ok(new ResponseBase
            {
                Retcode = -101,
                Message = "Account not found"
            });
        }
        
        if (!AccountData.VerifyPassword(accountData, password))
        {
            return Ok(new ResponseBase
            {
                Retcode = -201,
                Message = "Password incorrect"
            });
        }

        return Ok(new AppLoginByPasswordResponse
        {
            Retcode = 0,
            Message = "OK",
            Data = new AppLoginByPasswordResponse.AppLoginByPasswordAccountData
            {
                Token = new AppLoginByPasswordToken()
                {
                    Token = accountData.GenerateComboToken()
                },
                UserInfo = new AppLoginByPasswordUserInfo()
                {
                    aid = accountData.Uid.ToString(),
                    mid = accountData.Uid.ToString(),
                    AccountName = accountData.Username,
                    Email = $"{accountData!.Username}@neonteam.dev",
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
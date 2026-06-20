using System.Linq;
using System.Security.Cryptography;
using System.Text;
using NahidaImpact.Enums.Player;
using NahidaImpact.Util;
using NahidaImpact.Util.Extensions;
using NahidaImpact.Util.Security;
using SqlSugar;

namespace NahidaImpact.Database.Account;

[SugarTable("Account")]
public class AccountData : BaseDatabaseDataHelper
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";

    [SugarColumn(IsJson = true)] public List<PermEnum> Permissions { get; set; } = [];

    [SugarColumn(IsNullable = true)] public string? ComboToken { get; set; }

    #region GetAccount

    public static AccountData? GetAccountByUserName(string username)
    {
        var accounts = DatabaseHelper.GetAllInstance<AccountData>();
        return accounts?.FirstOrDefault(
            account => string.Equals(account.Username, username, StringComparison.OrdinalIgnoreCase));
    }

    public static AccountData? GetAccountByUid(int uid, bool force = false)
    {
        var result = DatabaseHelper.GetInstance<AccountData>(uid, force);
        return result;
    }

    #endregion

    #region Account

    public static void CreateAccount(string username, int uid, string password)
    {
        var newUid = uid;
        if (uid == 0)
        {
            var allAccounts = DatabaseHelper.GetAllInstance<AccountData>();
            newUid = allAccounts is { Count: > 0 }
                ? allAccounts.Max(a => a.Uid) + 1
                : 100001;
        }

        var account = new AccountData
        {
            Uid = newUid,
            Username = username,
            Password = "",
            Permissions = [.. ConfigManager.Config.ServerOption.DefaultPermissions
                .Select(perm => Enum.TryParse(perm, out PermEnum result) ? result : (PermEnum?)null)
                .Where(result => result.HasValue).Select(result => result!.Value)]
        };
        SetPassword(account, password);

        DatabaseHelper.CreateInstance(account);
    }

    public static void DeleteAccount(int uid)
    {
        if (GetAccountByUid(uid) == null) return;
        DatabaseHelper.DeleteAllInstance(uid);
    }

    public static void SetPassword(AccountData account, string password)
    {
        if (password.Length > 0)
            account.Password = Extensions.GetSha256Hash(password);
        else
            account.Password = "";
    }

    public static bool VerifyPassword(AccountData account, string password)
    {
        var hash = Extensions.GetSha256Hash(password ?? string.Empty);
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(account.Password), Encoding.UTF8.GetBytes(hash));
    }


    #endregion

    #region Permission

    public static bool HasPerm(PermEnum[] perms, int uid)
    {
        if (uid == (int)ServerEnum.Console) return true;
        var account = GetAccountByUid(uid);
        if (account?.Permissions == null) return false;
        if (account.Permissions.Contains(PermEnum.Admin)) return true;

        return perms.Any(account.Permissions.Contains);
    }

    public static void AddPerm(PermEnum[] perms, int uid)
    {
        if (uid == (int)ServerEnum.Console) return;
        var account = GetAccountByUid(uid);
        if (account == null) return;

        account.Permissions ??= [];
        foreach (var perm in perms)
        {
            if (!account.Permissions.Contains(perm))
            {
                account.Permissions = [.. account.Permissions, perm];
            }
        }
    }

    public static void RemovePerm(PermEnum[] perms, int uid)
    {
        if (uid == (int)ServerEnum.Console) return;
        var account = GetAccountByUid(uid);
        if (account == null) return;
        if (account.Permissions == null) return;

        foreach (var perm in perms)
        {
            if (account.Permissions.Contains(perm))
            {
                account.Permissions = account.Permissions.Except([perm]).ToList();
            }
        }
    }

    public static void CleanPerm(int uid)
    {
        if (uid == (int)ServerEnum.Console) return;
        var account = GetAccountByUid(uid);
        if (account == null) return;

        account.Permissions = [];
    }

    #endregion

    #region Token

    public string GenerateComboToken()
    {
        ComboToken = Crypto.CreateSessionKey(Uid.ToString());
        DatabaseHelper.UpdateInstance(this);
        return ComboToken;
    }

    #endregion

    #region Repository

    public static AccountData? FindAccountByUsername(string? username)
    {
        if (string.IsNullOrEmpty(username))
            return null;
        
        return AccountData.GetAccountByUserName(username);
    }

    public static async Task<(bool success, int accountUid)> CreateAccount(string? username, string? password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            return (false, 0);

        try
        {
            await Task.Run(() =>
            {
                AccountData.CreateAccount(username, 0, password);
            });

            var account = AccountData.GetAccountByUserName(username);
            if (account != null)
            {
                return (true, account.Uid);
            }

            return (false, 0);
        }
        catch
        {
            return (false, 0);
        }
    }

    public static AccountData? FindAccountByAccountUid(int accountUid)
    {
        if (accountUid <= 0)
            return null;

        return AccountData.GetAccountByUid(accountUid);
    }

    #endregion
}
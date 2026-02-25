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
    public BanTypeEnum BanType { get; set; }

    [SugarColumn(IsJson = true)] public List<PermEnum> Permissions { get; set; } = [];

    [SugarColumn(IsNullable = true)] public string? ComboToken { get; set; }

    #region GetAccount

    public static AccountData? GetAccountByUserName(string username)
    {
        AccountData? result = null;
        DatabaseHelper.GetAllInstance<AccountData>()?.ForEach(account =>
        {
            if (account.Username == username) result = account;
        });
        return result;
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
            newUid = 100001;
            while (GetAccountByUid(newUid) != null) newUid++;
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
        => account.Password == Extensions.GetSha256Hash(password);


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
}
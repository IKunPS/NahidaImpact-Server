using NahidaImpact.Database.Account;

namespace NahidaImpact.Database.Repositories;

public static class AccountRepository
{
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
}


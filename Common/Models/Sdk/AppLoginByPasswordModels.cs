namespace NahidaImpact.Models.Sdk;

public class AppLoginByPasswordResponse : ResponseBase
{
    public AppLoginByPasswordAccountData Data { get; set; } = new();

    public class AppLoginByPasswordAccountData
    {
        public AppLoginByPasswordToken Token { get; set; } = new();
        public AppLoginByPasswordUserInfo UserInfo { get; set; } = new();
    }
}

public class AppLoginByPasswordToken
{
    public int TokenType { get; set; } = 1;
    public string Token { get; set; }
}

public class AppLoginByPasswordUserInfo
{
    public string aid { get; set; }
    public string mid { get; set; }
    public string AccountName { get; set; }
    public string Email { get; set; }
    public int IsEmailVerify { get; set; }
}

public class AppLoginByPasswordRequest
{
    public string? Account { get; set; }
    public string? Password { get; set; }
}
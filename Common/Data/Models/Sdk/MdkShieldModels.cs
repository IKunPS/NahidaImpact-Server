namespace NahidaImpact.Data.Models.Sdk;

public class MdkShieldLoginRequest
{
    public string? Account { get; set; }
    public string? Password { get; set; }
    public bool IsCrypto { get; set; }
}

public class MdkShieldVerifyRequest
{
    public string? Uid { get; set; }
    public string? Token { get; set; }
}

// TODO: Move this to DB instead
public class MdkShieldAccountData
{
    public string? Token { get; set; }
    public string? Uid { get; set; }

    public string Email { get; set; } = "";
    public string IsEmailVerify { get; set; } = "0";
    public string AreaCode { get; set; } = "";
    public string Country { get; set; } = "";
    public string Name { get; set; } = "";
    public string Realname { get; set; } = "";
}

public class MdkShieldResponse : ResponseBase
{
    public new MdkShieldResponseData? Data { get; set; }

    public class MdkShieldResponseData
    {
        public MdkShieldAccountData? Account { get; set; }
        public bool DeviceGrantRequired { get; set; }
        public bool ReactiveRequired { get; set; }
        public bool RealpersonRequired { get; set; }
        public bool SafeMobileRequeired { get; set; }
    }
}

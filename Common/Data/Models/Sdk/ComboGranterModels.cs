using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using NahidaImpact.Util.Extensions;

namespace NahidaImpact.Data.Models.Sdk;

public class ComboGranterData
{
    public string? Uid { get; set; }
    public string? Token { get; set; }
}

public class ComboGranterRequest
{
    [Required]
    [JsonConverter(typeof(JsonStringToObjectConverter<ComboGranterData>))]
    public ComboGranterData? Data { get; set; }
}

public class ComboGranterResponse : ResponseBase
{
    public new ComboGranterResponseData? Data { get; set; }

    public class ComboGranterResponseData
    {
        public uint AccountType { get; set; }
        public string? OpenId { get; set; }
        public string? ComboId { get; set; }
        public string? ComboToken { get; set; }
        public bool Heartbeat { get; set; }
        public string? Data { get; set; }
    }
}


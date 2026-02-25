namespace NahidaImpact.Data.Models.Sdk;

public class ResponseBase
{
    public string Message { get; set; } = "OK";
    public bool Success { get; set; } = true;
    public int Retcode { get; set; }
    public object? Data { get; set; }
}
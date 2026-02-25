using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace NahidaImpact.Configuration;

public class HotfixContainer
{
    public bool UseLocalCache { get; set; } = false;
    public Dictionary<string, HotfixManfiset> Hotfixes { get; set; } = new();
}

public class HotfixManfiset
{
}
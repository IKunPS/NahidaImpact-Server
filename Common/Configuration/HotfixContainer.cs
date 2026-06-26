using Newtonsoft.Json;

namespace NahidaImpact.Configuration;

public class HotfixContainer
{
    // Key: full version name, e.g. "CNRELWin6.6.0", "OSRELiOS6.6.0"
    public Dictionary<string, HotfixManfiset> Hotfixes { get; set; } = new();
}

public class HotfixManfiset
{
    [JsonProperty("region_info")]
    public HotfixRegionInfo RegionInfo { get; set; } = new();
}

public class HotfixRegionInfo
{
    [JsonProperty("resource_url")]
    public string ResourceUrl { get; set; } = "";

    [JsonProperty("data_url")]
    public string DataUrl { get; set; } = "";

    [JsonProperty("client_data_md5")]
    public string ClientDataMd5 { get; set; } = "";

    [JsonProperty("client_silence_data_md5")]
    public string ClientSilenceDataMd5 { get; set; } = "";

    [JsonProperty("client_data_version")]
    public int ClientDataVersion { get; set; }

    [JsonProperty("client_silence_data_version")]
    public int ClientSilenceDataVersion { get; set; }

    [JsonProperty("client_version_suffix")]
    public string ClientVersionSuffix { get; set; } = "";

    [JsonProperty("client_silence_version_suffix")]
    public string ClientSilenceVersionSuffix { get; set; } = "";

    [JsonProperty("res_version_config")]
    public HotfixResVersionConfig ResVersionConfig { get; set; } = new();
}

public class HotfixResVersionConfig
{
    [JsonProperty("version")]
    public int Version { get; set; }

    [JsonProperty("md5")]
    public string Md5 { get; set; } = "";

    [JsonProperty("release_total_size")]
    public string ReleaseTotalSize { get; set; } = "";

    [JsonProperty("version_suffix")]
    public string VersionSuffix { get; set; } = "";

    [JsonProperty("branch")]
    public string Branch { get; set; } = "";
}

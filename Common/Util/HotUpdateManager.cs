using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using NahidaImpact.Configuration;
using NahidaImpact.Models.Sdk;
using NahidaImpact.Proto;
using NahidaImpact.Util.Security;
using Newtonsoft.Json;

namespace NahidaImpact.Util;

public static partial class HotUpdateManager
{
    private static readonly Logger Logger = new("HotUpdate");

    private static readonly HttpClient HttpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(10)
    };

    private static readonly ConcurrentDictionary<string, object> FetchLocks = new();

    private const string OsDispatchUrl = "https://osusadispatch.yuanshen.com/query_cur_region";
    private const string CnDispatchUrl = "https://cngfdispatch.yuanshen.com/query_cur_region";

    [GeneratedRegex(@"^(CN|OS)REL(Win|iOS|And)")]
    private static partial Regex VersionPrefixRegex();

    // Mirrors Java: HotUpdate.getHotUpdate() — returns RegionInfo (protobuf) or null
    public static RegionInfo? GetHotUpdate(string versionName, RegionInfo serverRegionInfo, string path, int keyId)
    {
        try
        {
            if (!ConfigManager.Hotfix.Hotfixes.TryGetValue(versionName, out var cached) ||
                cached.RegionInfo == null || string.IsNullOrEmpty(cached.RegionInfo.ResourceUrl))
            {
                // Cache miss — try auto-fetch for Android
                if (ConfigManager.Config.ServerOption.EnableHotUpdate &&
                    ConfigManager.Config.ServerOption.AutoCreateUser &&
                    versionName.Contains("Android"))
                {
                    var fetchLock = FetchLocks.GetOrAdd(versionName, _ => new object());
                    lock (fetchLock)
                    {
                        try
                        {
                            // Double-check after acquiring lock
                            if (ConfigManager.Hotfix.Hotfixes.TryGetValue(versionName, out var dblCached) &&
                                dblCached.RegionInfo != null && !string.IsNullOrEmpty(dblCached.RegionInfo.ResourceUrl))
                                return ReadFromCache(dblCached, serverRegionInfo);

                            return FetchAndCache(versionName, path, keyId);
                        }
                        finally
                        {
                            FetchLocks.TryRemove(versionName, out _);
                        }
                    }
                }

                throw new InvalidOperationException($"Hotfix not available for {versionName}");
            }

            return ReadFromCache(cached, serverRegionInfo);
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to get hotfix for {versionName}: {ex}");
            return null;
        }
    }

    // Build RegionInfo from cached JSON — local gate IP/port, cached resource URLs
    private static RegionInfo ReadFromCache(HotfixManfiset manifest, RegionInfo serverRegionInfo)
    {
        var ri = manifest.RegionInfo;
        var regionInfo = new RegionInfo
        {
            GateserverIp = serverRegionInfo.GateserverIp,
            GateserverPort = serverRegionInfo.GateserverPort,
            ResourceUrl = ri.ResourceUrl,
            DataUrl = ri.DataUrl,
            ClientDataMd5 = ri.ClientDataMd5,
            ClientSilenceDataMd5 = ri.ClientSilenceDataMd5,
            ClientDataVersion = (uint)ri.ClientDataVersion,
            ClientSilenceDataVersion = (uint)ri.ClientSilenceDataVersion,
            ClientVersionSuffix = ri.ClientVersionSuffix,
            ClientSilenceVersionSuffix = ri.ClientSilenceVersionSuffix,
            ResVersionConfig = new ResVersionConfig
            {
                Version = (uint)ri.ResVersionConfig.Version,
                Md5 = ri.ResVersionConfig.Md5,
                ReleaseTotalSize = ri.ResVersionConfig.ReleaseTotalSize,
                VersionSuffix = ri.ResVersionConfig.VersionSuffix,
                Branch = ri.ResVersionConfig.Branch
            }
        };
        return regionInfo;
    }

    // Fetch from official dispatch, cache, return RegionInfo
    private static RegionInfo? FetchAndCache(string versionName, string path, int keyId)
    {
        var baseUrl = versionName.Contains("CN") ? CnDispatchUrl : OsDispatchUrl;
        var url = $"{baseUrl}?{path}";

        Logger.Info($"Fetching hotfix: {baseUrl}");

        var httpResponse = RunSync(() => HttpClient.GetAsync(url));
        httpResponse.EnsureSuccessStatusCode();
        var responseJson = RunSync(() => httpResponse.Content.ReadAsStringAsync());

        var root = JsonConvert.DeserializeObject<QueryCurRegionRspJson>(responseJson);
        if (root?.Content == null)
            throw new InvalidOperationException("Dispatch response content is null");

        var encrypted = Convert.FromBase64String(root.Content);
        var decrypted = Crypto.DecryptRegionData(encrypted, keyId);
        var parsed = QueryCurrRegionHttpRsp.Parser.ParseFrom(decrypted);
        var reInfo = parsed.RegionInfo;

        var manifest = new HotfixManfiset
        {
            RegionInfo = new HotfixRegionInfo
            {
                ResourceUrl = reInfo.ResourceUrl,
                DataUrl = reInfo.DataUrl,
                ClientDataMd5 = reInfo.ClientDataMd5,
                ClientSilenceDataMd5 = reInfo.ClientSilenceDataMd5,
                ClientDataVersion = (int)reInfo.ClientDataVersion,
                ClientSilenceDataVersion = (int)reInfo.ClientSilenceDataVersion,
                ClientVersionSuffix = reInfo.ClientVersionSuffix,
                ClientSilenceVersionSuffix = reInfo.ClientSilenceVersionSuffix,
                ResVersionConfig = new HotfixResVersionConfig
                {
                    Version = (int)reInfo.ResVersionConfig.Version,
                    Md5 = reInfo.ResVersionConfig.Md5,
                    ReleaseTotalSize = reInfo.ResVersionConfig.ReleaseTotalSize,
                    VersionSuffix = reInfo.ResVersionConfig.VersionSuffix,
                    Branch = reInfo.ResVersionConfig.Branch
                }
            }
        };

        ConfigManager.Hotfix.Hotfixes[versionName] = manifest;
        ConfigManager.SaveHotfix();

        Logger.Info($"Hotfix cached for {versionName}");
        return reInfo;
    }

    // Run async on thread pool — avoids ASP.NET sync-context deadlock
    private static T RunSync<T>(Func<Task<T>> task) => Task.Run(task).GetAwaiter().GetResult();
}

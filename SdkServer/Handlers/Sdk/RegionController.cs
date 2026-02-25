using Google.Protobuf;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using NahidaImpact.Data.Models.Dispatch;
using NahidaImpact.Util;
using NahidaImpact.Util.Security;
using NahidaImpact.Configuration;

namespace NahidaImpact.SdkServer.Handlers.Sdk;

[ApiController]
public class RegionController : ControllerBase
{
    private const string DefaultRegionKey = "os_usa";
    private const string DefaultRegionData = "CAESGE5vdCBGb3VuZCB2ZXJzaW9uIGNvbmZpZw==";

    private static readonly int[] ServerVersionParts =
        GameConstants.GAME_VERSION.Split('.').Select(int.Parse).ToArray();

    private static readonly Dictionary<string, RegionData> Regions;

    // 缓存 CN 和 OS 区域列表的响应
    private static string _regionListResponseOs = string.Empty;
    private static string _regionListResponseCn = string.Empty;

    static RegionController()
    {
        Regions = new Dictionary<string, RegionData>
        {
            { DefaultRegionKey, new RegionData(BuildDefaultRegionResponse()) },
        };
        
        InitializeRegionList();
    }
    
    private static void InitializeRegionList()
    {
        var dispatchDomain = ConfigManager.Config.HttpServer.GetDisplayAddress();

        var servers = BuildRegionServerList(dispatchDomain);

        // OS 配置
        var osConfig = BuildRegionClientConfig("2");
        _regionListResponseOs = BuildRegionListResponse(osConfig, servers);
        
        // CN 配置
        var cnConfig = BuildRegionClientConfig("0");
        _regionListResponseCn = BuildRegionListResponse(cnConfig, servers);
    }

    private static List<RegionSimpleInfo> BuildRegionServerList(string dispatchDomain)
    {
        return
        [
            new RegionSimpleInfo
            {
                Type = "DEV_PUBLIC",
                DispatchUrl = $"{dispatchDomain}/query_cur_region/",
                Name = ConfigManager.Config.Region.Name,
                Title = ConfigManager.Config.Region.Title
            }
        ];
    }

    private static object BuildRegionClientConfig(string sdkenv)
    {
        return new
        {
            sdkenv,
            checkdevice = "false",
            loadPatch = "false",
            showexception = "false",
            regionConfig = "pm|fk|add",
            downloadMode = "0",
            codeSwitch = "4334", // 4.6及以上版本
            coverSwitch = "40"
        };
    }

    private static string BuildRegionListResponse(object clientConfig, List<RegionSimpleInfo> servers)
    {
        var configJson = System.Text.Json.JsonSerializer.Serialize(clientConfig);
        var configEncrypted = Crypto.Xor(configJson, Crypto.DISPATCH_KEY);

        QueryRegionListHttpRsp rsp = new()
        {
            ClientCustomConfigEncrypted = ByteString.CopyFrom(configEncrypted),
            ClientSecretKey = ByteString.CopyFrom(Crypto.DISPATCH_SEED),
            EnableLoginPc = true,
            Retcode = (int)Retcode.RetSucc
        };
        
        rsp.RegionList.AddRange(servers);
        return Convert.ToBase64String(rsp.ToByteArray());
    }

    [HttpGet("/query_cur_region")]
    public IActionResult QueryCurRegion([FromQuery] DispatchQuery query, Logger logger)
    {
        var version = query.Version;
        var keyId = query.Key_Id;
        var dispatchSeed = query.DispatchSeed;
        const string region = DefaultRegionKey;

        // 获取区域数据（若不存在则返回默认配置）
        var regionData = GetRegionBase64(region);
        
        try
        {
            // 清理并解析版本号
            var clientVersionClean = Regex.Replace(version!, "[a-zA-Z]", "");
            var versionCode = clientVersionClean.Split('.');
            if (versionCode.Length < 3)
            {
                return Ok(regionData);
            }

            if (ConfigManager.Config.ServerOption.IsServerStop)
            {
                var stopServer = new StopServerInfo
                {
                    StopBeginTime = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    StopEndTime = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 18000,
                };

                var rsp = new QueryCurrRegionHttpRsp
                {
                    Retcode = (int)Retcode.RetStopServer,
                    Msg = "服务器维护",
                    RegionInfo = new RegionInfo(),
                    StopServer = stopServer
                };
                    
                var encryptedResponse = Crypto.EncryptAndSignRegionData(rsp.ToByteArray(), keyId);
                return Ok(encryptedResponse);
            }

            var (versionMajor, versionMinor, versionFix) = ParseClientVersion(versionCode);

            // 新客户端处理逻辑
            if (IsNewClient(versionMajor, versionMinor, versionFix))
            {
                // 版本不匹配检查
                if (IsVersionMismatch(versionMajor, versionMinor, clientVersionClean))
                {
                    var encryptedResponse = BuildVersionMismatchEncryptedResponse(clientVersionClean, keyId);
                    return Ok(encryptedResponse);
                }

                // UA Patch
                if (dispatchSeed == null)
                {
                    return Ok(new 
                    { 
                        content = regionData,
                        sign = "TW9yZSBsb3ZlIGZvciBVQSBQYXRjaCBwbGF5ZXJz" 
                    });
                }

                // Encryption and Signature
                var encrypted = EncryptRegionData(regionData, keyId);
                return Ok(encrypted);
            }

            // 旧版本客户端处理
            return Ok(regionData);
        }
        catch (Exception e)
        {
            logger.Error($"Error handling query_cur_region: {e}");
            return Ok(regionData);
        }
    }

    [HttpGet("/query_region_list")] [HttpHead("/query_region_list")]
    public IActionResult QueryRegionList([FromQuery] DispatchQuery query)
    {
        var versionName = query.Version;
        
        // 根据版本号前缀判断使用 CN 还是 OS 的区域列表
        var targetRegionList = DetermineRegionListByVersion(versionName);
        return Ok(targetRegionList);
    }

    private static QueryCurrRegionHttpRsp BuildDefaultRegionResponse()
    {
        var cfg = ConfigManager.Config.Region;

        return new QueryCurrRegionHttpRsp
        {
            Retcode = (int)Retcode.RetSucc,
            ClientSecretKey = ByteString.CopyFrom(Crypto.DISPATCH_SEED),
            RegionInfo = new RegionInfo
            {
                GateserverIp = cfg.Ip,
                GateserverPort = cfg.Port
            }
        };
    }

    private static string GetRegionBase64(string region)
    {
        if (Regions.TryGetValue(region, out var regionObj))
        {
            return regionObj.Base64;
        }

        return DefaultRegionData;
    }

    private static (int Major, int Minor, int Fix) ParseClientVersion(string[] versionCode)
    {
        var major = int.Parse(versionCode[0]);
        var minor = int.Parse(versionCode[1]);
        var fix = int.Parse(versionCode[2]);
        return (major, minor, fix);
    }

    private static bool IsNewClient(int major, int minor, int fix)
    {
        return major >= 3 ||
               (major == 2 && minor == 7 && fix >= 50) ||
               (major == 2 && minor == 8);
    }

    private static bool IsVersionMismatch(int clientMajor, int clientMinor, string clientVersionClean)
    {
        return clientMajor != ServerVersionParts[0] || clientMinor != ServerVersionParts[1];
    }

    private static object BuildVersionMismatchEncryptedResponse(string clientVersionClean, string? keyId)
    {
        var updateClient = string.Compare(GameConstants.GAME_VERSION, clientVersionClean, StringComparison.Ordinal) > 0;

        var contentMsg = updateClient
            ? $"\n版本不匹配 过时的客户端! \n\nServer version: {GameConstants.GAME_VERSION}\nClient version: {clientVersionClean}"
            : $"\n版本不匹配 过时的服务器! \n\nServer version: {GameConstants.GAME_VERSION}\nClient version: {clientVersionClean}";

        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        var stopServer = new StopServerInfo
        {
            Url = "https://www.bilibili.com/video/BV1GJ411x7h7/",
            StopBeginTime = (uint)now,
            StopEndTime = (uint)(now + 1000),
            ContentMsg = contentMsg
        };

        var rsp = new QueryCurrRegionHttpRsp
        {
            Retcode = (int)Retcode.RetStopServer,
            Msg = "Connection Failed!",
            RegionInfo = new RegionInfo(),
            StopServer = stopServer
        };

        return Crypto.EncryptAndSignRegionData(rsp.ToByteArray(), keyId);
    }

    private static object EncryptRegionData(string regionData, string? keyId)
    {
        var regionInfo = Convert.FromBase64String(regionData);
        return Crypto.EncryptAndSignRegionData(regionInfo, keyId);
    }

    private static string DetermineRegionListByVersion(string? versionName)
    {
        if (string.IsNullOrEmpty(versionName))
        {
            return _regionListResponseOs;
        }

        string versionCode;
        try
        {
            versionCode = versionName.Length >= 8 ? versionName[..8] : versionName;
        }
        catch
        {
            versionCode = versionName;
        }

        // CN 客户端
        if ("CNRELiOS".Equals(versionCode) ||
            "CNRELWin".Equals(versionCode) ||
            "CNRELAnd".Equals(versionCode))
        {
            return _regionListResponseCn;
        }

        // OS 客户端
        if ("OSRELiOS".Equals(versionCode) ||
            "OSRELWin".Equals(versionCode) ||
            "OSRELAnd".Equals(versionCode))
        {
            return _regionListResponseOs;
        }

        // 默认返回 OS 区域列表
        return _regionListResponseOs;
    }

}
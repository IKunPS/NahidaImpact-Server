using Google.Protobuf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using NahidaImpact.Data.Models.Dispatch;
using NahidaImpact.Enums.Player;
using NahidaImpact.Util;
using NahidaImpact.Util.Security;

namespace NahidaImpact.SdkServer.Handlers.Sdk;

[ApiController]
public class RegionController : ControllerBase
{
    private static readonly int[] ServerVersionParts =
        GameConstants.GAME_VERSION.Split('.').Select(int.Parse).ToArray();

    private static readonly IReadOnlyDictionary<string, RegionData> Regions;
    private static readonly string RegionListResponseOs;
    private static readonly string RegionListResponseCn;
    
    private static readonly HashSet<string> CnVersionPrefixes = new()
    {
        "CNRELiOS", "CNRELWin", "CNRELAnd"
    };
    
    private readonly ILogger<RegionController> _logger;

    static RegionController()
    {
        var regions = new Dictionary<string, RegionData>
        {
            { ConfigManager.Config.Region.Name, new RegionData(GetDefaultRegionResponse()) }
        };
        Regions = regions;
        
        var dispatchDomain = ConfigManager.Config.HttpServer.GetDisplayAddress();
        var servers = GetRegionServerList(dispatchDomain);

        var osConfig = GetRegionClientConfig(BaseRegionEnum.OS);
        RegionListResponseOs = GetRegionListResponse(osConfig, servers);

        var cnConfig = GetRegionClientConfig(BaseRegionEnum.CN);
        RegionListResponseCn = GetRegionListResponse(cnConfig, servers);
    }

    public RegionController(ILogger<RegionController> logger)
    {
        _logger = logger;
    }
    
    private static List<RegionSimpleInfo> GetRegionServerList(string dispatchDomain)
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

    private static object GetRegionClientConfig(BaseRegionEnum sdkenv)
    {
        return new
        {
            sdkenv = (int)sdkenv,
            checkdevice = "false",
            loadPatch = "false",
            showexception = "false",
            regionConfig = "pm|fk|add",
            downloadMode = "0",
            codeSwitch = "4334",
            coverSwitch = "40"
        };
    }

    private static string GetRegionListResponse(object clientConfig, List<RegionSimpleInfo> servers)
    {
        var configJson = System.Text.Json.JsonSerializer.Serialize(clientConfig);
        var configEncrypted = Crypto.Xor(configJson, Crypto.DISPATCH_KEY);

        var rsp = new QueryRegionListHttpRsp
        {
            ClientCustomConfigEncrypted = ByteString.CopyFrom(configEncrypted),
            ClientSecretKey = ByteString.CopyFrom(Crypto.DISPATCH_SEED),
            EnableLoginPc = true,
            Retcode = (int)Retcode.RetSucc
        };
        rsp.RegionList.AddRange(servers);
        return Convert.ToBase64String(rsp.ToByteArray());
    }

    private static QueryCurrRegionHttpRsp GetDefaultRegionResponse()
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
        return Regions.TryGetValue(region, out var regionObj) ? regionObj.Base64 : "CAESGE5vdCBGb3VuZCB2ZXJzaW9uIGNvbmZpZw==";
    }

    private static (int Major, int Minor, int Fix) ParseClientVersion(string[] versionCode)
    {
        // 假设 versionCode 长度已确保 >=3，此处不做额外检查（外层已处理）
        return (int.Parse(versionCode[0]), int.Parse(versionCode[1]), int.Parse(versionCode[2]));
    }

    private static bool IsNewClient(int major, int minor, int fix)
    {
        return major >= 3 ||
               (major == 2 && minor == 7 && fix >= 50) ||
               (major == 2 && minor == 8);
    }

    private static bool IsVersionMismatch(int clientMajor, int clientMinor)
    {
        return clientMajor != ServerVersionParts[0] || clientMinor != ServerVersionParts[1];
    }

    private static object GetVersionMismatchEncryptedResponse(string clientVersionClean, string? keyId)
    {
        bool updateClient = string.Compare(GameConstants.GAME_VERSION, clientVersionClean, StringComparison.Ordinal) > 0;
        string contentMsg = updateClient
            ? $"\n版本不匹配 过时的客户端! \n\nServer version: {GameConstants.GAME_VERSION}\nClient version: {clientVersionClean}"
            : $"\n版本不匹配 过时的服务器! \n\nServer version: {GameConstants.GAME_VERSION}\nClient version: {clientVersionClean}";

        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
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
        byte[] regionInfo = Convert.FromBase64String(regionData);
        return Crypto.EncryptAndSignRegionData(regionInfo, keyId);
    }

    [HttpGet("/query_cur_region")]
    public IActionResult QueryCurRegion([FromQuery] DispatchQuery query)
    {
        string? version = query.Version;
        string? keyId = query.Key_Id;
        string? dispatchSeed = query.DispatchSeed;

        string regionData = GetRegionBase64(ConfigManager.Config.Region.Name);
        
        if (string.IsNullOrEmpty(version))
        {
            return Ok(regionData);
        }

        try
        {
            string clientVersionClean = Regex.Replace(version, "[a-zA-Z]", "");
            string[] versionCode = clientVersionClean.Split('.');
            if (versionCode.Length < 3)
            {
                return Ok(regionData);
            }

            var (major, minor, fix) = ParseClientVersion(versionCode);

            if (IsNewClient(major, minor, fix))
            {
                if (IsVersionMismatch(major, minor))
                {
                    return Ok(GetVersionMismatchEncryptedResponse(clientVersionClean, keyId));
                }

                if (dispatchSeed == null)
                {
                    return Ok(new
                    {
                        content = regionData,
                        sign = "TW9yZSBsb3ZlIGZvciBVQSBQYXRjaCBwbGF5ZXJz"
                    });
                }

                return Ok(EncryptRegionData(regionData, keyId));
            }

            return Ok(regionData);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error handling query_cur_region for version {Version}", version);
            return Ok(regionData);
        }
    }

    [HttpGet("/query_region_list")]
    public IActionResult QueryRegionList([FromQuery] DispatchQuery query)
    {
        string? versionName = query.Version;

        if (string.IsNullOrEmpty(versionName))
        {
            return Ok(RegionListResponseOs);
        }

        string versionCode = versionName.Length >= 8 ? versionName[..8] : versionName;

        if (CnVersionPrefixes.Contains(versionCode))
        {
            return Ok(RegionListResponseCn);
        }
        
        return Ok(RegionListResponseOs);
    }
}
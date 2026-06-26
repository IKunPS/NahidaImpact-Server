using Google.Protobuf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using NahidaImpact.Internationalization;
using NahidaImpact.Models.Dispatch;
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
        
        var dispatchDomain = ConfigManager.Config.HttpServer.DisplayAddress;
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

    private static bool IsVersionMismatch(int clientMajor, int clientMinor)
    {
        return clientMajor != ServerVersionParts[0] || clientMinor != ServerVersionParts[1];
    }

    private static object EncryptRegionData(byte[] regionInfo, string? keyId)
    {
        return Crypto.EncryptAndSignRegionData(regionInfo, keyId);
    }

    // Line-for-line match of Java: RegionHandler.queryCurrentRegion
    [HttpGet("/query_cur_region")]
    public IActionResult QueryCurRegion([FromQuery] DispatchQuery query)
    {
        // ctx.pathParam("region") + ctx.queryParam("version")
        string? versionName = query.Version;
        string? keyId = query.Key_Id;
        string? dispatchSeed = query.DispatchSeed;

        // regionData fallback
        string regionData = GetRegionBase64(ConfigManager.Config.Region.Name);

        // Old client: missing versionName / dispatchSeed / key_id → JSON with sign marker
        if (string.IsNullOrEmpty(versionName) || dispatchSeed == null || keyId == null)
        {
            return Ok(new
            {
                content = regionData,
                sign = "TW9yZSBsb3ZlIGZvciBVQSBQYXRjaCBwbGF5ZXJz"
            });
        }

        // Parse version: VERSION_LETTERS.matcher(versionName).replaceAll("")
        string clientVersion = Regex.Replace(versionName, "[a-zA-Z]", "");
        string[] versionCode = clientVersion.Split('.');
        if (versionCode.Length < 3) return Ok(regionData);

        int versionMajor = int.Parse(versionCode[0]);
        int versionMinor = int.Parse(versionCode[1]);
        // _versionFix not checked (matches Java)

        // Version mismatch → force-update (ForceUdpate field, not StopServer)
        if (versionMajor != ServerVersionParts[0] || versionMinor != ServerVersionParts[1])
        {
            bool updateClient = string.Compare(GameConstants.GAME_VERSION, clientVersion, StringComparison.Ordinal) > 0;
            string contentMsg = updateClient
                ? I18NManager.Translate("Server.Web.ClientOutdated", GameConstants.GAME_VERSION, clientVersion)
                : I18NManager.Translate("Server.Web.ServerOutdated", GameConstants.GAME_VERSION, clientVersion);

            var rsp = new QueryCurrRegionHttpRsp
            {
                Retcode = (int)Retcode.RetClientForceUpdate,
                Msg = contentMsg,
                RegionInfo = new RegionInfo(),
                ForceUdpate = new ForceUpdateInfo
                {
                    ForceUpdateUrl = "http://mynxy.cn"
                }
            };

            return Ok(EncryptRegionData(rsp.ToByteArray(), keyId));
        }

        try
        {
            var serverRegionInfo = Regions[ConfigManager.Config.Region.Name].GetRegionQuery().RegionInfo;
            var queryStr = Request.QueryString.Value?.TrimStart('?') ?? "";
            var hotUpdateRegionInfo = HotUpdateManager.GetHotUpdate(
                versionName, serverRegionInfo, queryStr, int.Parse(keyId));

            // retcode + stopServer: starts as success/default, overridden if IsServerStop
            int retcode = (int)Retcode.RetSucc;
            StopServerInfo stopServerInfo = new();
            string stopMsg = I18NManager.Translate("Server.Web.Maintain");
            if (ConfigManager.Config.ServerOption.IsServerStop)
            {
                retcode = (int)Retcode.RetStopServer;
                long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                stopServerInfo = new StopServerInfo
                {
                    Url = "https://www.bilibili.com/video/BV18AyoBXEvD",
                    StopBeginTime = (uint)now,
                    StopEndTime = (uint)(now + 18000),
                    ContentMsg = stopMsg
                };
            }

            byte[] regionInfoBytes;

            if (ConfigManager.Config.ServerOption.EnableHotUpdate && hotUpdateRegionInfo != null)
            {
                regionInfoBytes = new QueryCurrRegionHttpRsp
                {
                    RegionInfo = hotUpdateRegionInfo,
                    ClientSecretKey = ByteString.CopyFrom(Crypto.DISPATCH_SEED),
                    StopServer = stopServerInfo,
                    Msg = stopMsg,
                    Retcode = retcode
                }.ToByteArray();
            }
            else if (ConfigManager.Config.ServerOption.IsServerStop)
            {
                var defaultRsp = QueryCurrRegionHttpRsp.Parser.ParseFrom(
                    Convert.FromBase64String(regionData));
                defaultRsp.StopServer = stopServerInfo;
                defaultRsp.Msg = stopMsg;
                defaultRsp.Retcode = retcode;
                regionInfoBytes = defaultRsp.ToByteArray();
            }
            else
            {
                regionInfoBytes = Convert.FromBase64String(regionData);
            }

            return Ok(EncryptRegionData(regionInfoBytes, keyId));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error handling query_cur_region for version {Version}", versionName);
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
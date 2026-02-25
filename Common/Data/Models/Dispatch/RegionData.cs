using Google.Protobuf;

namespace NahidaImpact.Data.Models.Dispatch;

public class RegionData
{
    private readonly QueryCurrRegionHttpRsp _regionQuery;
    public string Base64 => Convert.ToBase64String(_regionQuery.ToByteArray());

    public RegionData(QueryCurrRegionHttpRsp regionQuery)
    {
        _regionQuery = regionQuery;
    }
}
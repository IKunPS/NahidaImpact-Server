namespace NahidaImpact.Data.Models.Sdk;

public class GetWeatherResponse : ResponseBase
{
    public new GetWeatherResponseData? Data { get; set; }

    public class GetWeatherResponseData
    {
        public int Timezone { get; set; }
        public List<HourlyWeatherData> Hourly { get; set; } = [];

        public class HourlyWeatherData
        {
            public int Condition { get; set; }
            public int Hour { get; set; }
            public string? Date { get; set; }
            public int Temp { get; set; }
        }
    }
}

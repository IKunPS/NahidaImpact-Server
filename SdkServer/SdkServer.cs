using NahidaImpact.Util;
using Microsoft.AspNetCore;
using System.Text.Json;

namespace NahidaImpact.SdkServer;

public class SdkServer
{
    public static void Main(string[] args)
    {
        BuildWebHost(args).Start();
    }

    private static IWebHost BuildWebHost(string[] args)
    {
        var builder = WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .ConfigureLogging((_, logging) => { logging.ClearProviders(); })
            .UseUrls(ConfigManager.Config.HttpServer.GetBindDisplayAddress());

        return builder.Build();
    }
}

public class Startup
{
    public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

        app.UseRouting();
        app.UseCors("AllowAll");
        app.UseAuthorization();
        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }

    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
        });
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
            });
        services.AddSingleton<Logger>(_ => new Logger("HttpServer"));
    }
}
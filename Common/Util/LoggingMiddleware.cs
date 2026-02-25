using NahidaImpact.Util;
using Microsoft.AspNetCore.Http;

namespace NahidaImpact.Util;

public class RequestLoggingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, Logger logger)
    {
        var request = context.Request;
        var method = request.Method;
        var path = request.Path + request.QueryString;

        await next(context);

        var statusCode = context.Response.StatusCode;

        if (path.StartsWith("/report") || path.Contains("/log/") || path == "/alive")
            return;

        if (statusCode == 200)
        {
            logger.Info($"{method} {path} => {statusCode}");
        }
        else if (statusCode == 404)
        {
            logger.Warn($"{method} {path} => {statusCode}");
        }
        else
        {
            logger.Error($"{method} {path} => {statusCode}");
        }
    }
}
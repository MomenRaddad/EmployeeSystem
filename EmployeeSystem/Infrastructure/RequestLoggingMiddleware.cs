using System.Diagnostics;

namespace EmployeeSystem.Infrastructure;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{

    public async Task InvokeAsync(HttpContext context)
    {
        var method = context.Request.Method;
        var path = context.Request.Path;

        var stopwatch = Stopwatch.StartNew();

        logger.LogInformation("[REQ] Incoming HTTP {Method} {Path}", method, path);

        await next(context);

        stopwatch.Stop();
        var statusCode = context.Response.StatusCode;
        var elapsedMs = stopwatch.ElapsedMilliseconds;

        if (statusCode >= 400)
        {
            logger.LogWarning(
                "[REQ] HTTP {Method} {Path} responded {StatusCode} in {Elapsed} ms",
                method,
                path,
                statusCode,
                elapsedMs);
        }
        else
        {
            logger.LogInformation(
                "[REQ] HTTP {Method} {Path} responded {StatusCode} in {Elapsed} ms",
                method,
                path,
                statusCode,
                elapsedMs);
        }
    }
}

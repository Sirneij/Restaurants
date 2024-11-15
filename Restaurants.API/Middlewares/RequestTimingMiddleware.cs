using System.Diagnostics;

namespace Restaurants.API.Middlewares;
public class RequestTimingMiddleware(
    ILogger<RequestTimingMiddleware> logger
) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var stopwatch = Stopwatch.StartNew();

        await next.Invoke(context);

        stopwatch.Stop();

        var elapsed = stopwatch.ElapsedMilliseconds;

        if (elapsed > 1000)
        {
            logger.LogWarning("Request [{path}] {method} took {elapsed} ms", context.Request.Path, context.Request.Method, elapsed);
        }

    }
}

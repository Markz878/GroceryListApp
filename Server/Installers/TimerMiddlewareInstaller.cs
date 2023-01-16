namespace GroceryListHelper.Server.Installers;

public sealed class TimerMiddlewareInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddScoped<TimerMiddleware>();
        }
    }
}

internal class TimerMiddleware : IMiddleware
{
    private readonly ILogger<TimerMiddleware> logger;

    public TimerMiddleware(ILogger<TimerMiddleware> logger)
    {
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Path.StartsWithSegments(new PathString("/api")))
        {
            long timestamp = Stopwatch.GetTimestamp();
            await next(context);
            TimeSpan elapsedTime = Stopwatch.GetElapsedTime(timestamp);
            logger.LogInformation("Request to {path} took {elapsedMilliseconds} ms.", context.Request.Path, elapsedTime.TotalMilliseconds);
        }
        else
        {
            await next(context);
        }
    }
}

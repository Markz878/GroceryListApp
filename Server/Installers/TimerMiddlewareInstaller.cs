namespace GroceryListHelper.Server.Installers;

public sealed class TimerMiddlewareInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddTransient<TimerMiddleware>();
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
            Stopwatch stopwatch = Stopwatch.StartNew();
            await next(context);
            stopwatch.Stop();
            logger.LogInformation("Request to {path} took {elapsedMilliseconds} ms.", context.Request.Path, stopwatch.ElapsedMilliseconds);
        }
        else
        {
            await next(context);
        }
    }
}

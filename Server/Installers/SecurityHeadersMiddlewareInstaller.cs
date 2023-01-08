namespace GroceryListHelper.Server.Installers;

public sealed class SecurityHeadersMiddlewareInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);
        builder.Services.AddSingleton<SecurityHeadersMiddleware>();
    }
}

public sealed class SecurityHeadersMiddleware : IMiddleware
{
    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        context.Response.Headers.Add("X-Content-Type-Options", new StringValues("nosniff"));
        context.Response.Headers.Add("X-Frame-Options", new StringValues("DENY"));
        //context.Response.Headers.Add("Content-Security-Policy", new StringValues("default-src 'self' https:; script-src 'self' 'unsafe-eval' 'unsafe-inline' https://kit.fontawesome.com/52e1a78681.js; style-src 'unsafe-inline' 'self' https:"));
        context.Response.Headers.Add("Referrer-Policy", new StringValues("no-referrer"));
        //context.Response.Headers.Add("Permissions-Policy", new StringValues("sync-xhr 'none'"));
        context.Response.Headers.Add("X-XSS-Protection", new StringValues("1; mode=block"));
        context.Response.Headers.Add("Cross-Origin-Opener-Policy", new StringValues("same-origin"));
        return next(context);
    }
}

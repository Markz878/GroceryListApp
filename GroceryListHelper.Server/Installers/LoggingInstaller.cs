using GroceryListHelper.Server.HelperMethods;
using Microsoft.AspNetCore.HttpLogging;

namespace GroceryListHelper.Server.Installers;

public class LoggingInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Services.AddHttpLogging(logging =>
        {
            logging.CombineLogs = true;
            logging.LoggingFields = HttpLoggingFields.RequestMethod | HttpLoggingFields.ResponseStatusCode;
        });
        if (builder.Configuration.GetValue<bool>("AddLogging"))
        {
            builder.Logging.AddSimpleConsole(x =>
            {
                x.UseUtcTimestamp = true;
                x.TimestampFormat = "dd/MM/yy HH:mm:ss ";
                x.SingleLine = builder.Environment.IsProduction();
            });
            builder.Services.AddHttpLoggingInterceptor<HttpLoggingInterceptor>();
            if (builder.Environment.IsProduction())
            {
                builder.Services.AddApplicationInsightsTelemetry(x =>
                {
                    x.EnableDependencyTrackingTelemetryModule = false;
                    x.Credential = new ManagedIdentityCredential(ManagedIdentityId.SystemAssigned);
                });
            }
        }
    }
}

public class HttpLoggingInterceptor : IHttpLoggingInterceptor
{
    public ValueTask OnRequestAsync(HttpLoggingInterceptorContext logContext)
    {
        string? user = logContext.HttpContext.User.FindFirstValue(AuthenticationConstants.EmailClaimName);
        string? path = logContext.HttpContext.Request?.Path.Value;
        string? query = logContext.HttpContext.Request?.QueryString.Value;
        if (!string.IsNullOrWhiteSpace(user))
        {
            logContext.AddParameter("User", user);
        }
        if (!string.IsNullOrWhiteSpace(path))
        {
            logContext.AddParameter("Path", path);
        }
        if (!string.IsNullOrWhiteSpace(query))
        {
            logContext.AddParameter("Query", query);
        }
        return default;
    }

    public ValueTask OnResponseAsync(HttpLoggingInterceptorContext logContext)
    {
        return default;
    }
}
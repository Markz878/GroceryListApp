using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
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
            logging.LoggingFields = HttpLoggingFields.All;
        });
        if (builder.Configuration.GetValue<bool>("AddLogging"))
        {
            builder.Logging.AddSimpleConsole(x =>
            {
                x.UseUtcTimestamp = true;
                x.TimestampFormat = "dd/MM/yy HH:mm:ss ";
            });
            builder.Logging.AddApplicationInsights();
            builder.Services.AddApplicationInsightsTelemetry(x => x.EnableDependencyTrackingTelemetryModule = false);
            builder.Services.AddApplicationInsightsTelemetryProcessor<IgnoreRequestPathsTelemetryProcessor>();
            builder.Services.Configure<TelemetryConfiguration>(c =>
            {
                c.SetAzureTokenCredential(new ManagedIdentityCredential());
            });
        }
    }
}

public class IgnoreRequestPathsTelemetryProcessor(ITelemetryProcessor next) : ITelemetryProcessor
{
    private readonly ITelemetryProcessor _next = next;

    public void Process(ITelemetry telemetry)
    {
        if (telemetry is RequestTelemetry requestTelemetry)
        {
            if (SkipTelemetry(requestTelemetry.Url.AbsolutePath))
            {
                return;
            }
        }
        _next.Process(telemetry);
    }

    private static readonly string[] _ignorePaths = ["/health", "/favicon.ico", "_framework/opaque-redirect"];
    private static readonly string[] _fileEndings = [".br", ".js", ".svg", ".png", ".css", ".json"];
    private static bool SkipTelemetry(string path)
    {
        if (_ignorePaths.Contains(path))
        {
            return true;
        }
        foreach (string fileEnding in _fileEndings)
        {
            if (path.EndsWith(fileEnding))
            {
                return true;
            }
        }
        return false;
    }
}

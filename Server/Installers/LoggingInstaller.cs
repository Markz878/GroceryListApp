using Microsoft.AspNetCore.HttpLogging;
using Serilog;

namespace GroceryListHelper.Server.Installers;

public class LoggingInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        if (builder.Environment.IsProduction())
        {
            builder.Logging.AddApplicationInsights();
        }
        if (builder.Configuration.GetValue<bool>("AddLogging"))
        {
            builder.Host.UseSerilog((context, services, configuration) =>
            {
                configuration.ReadFrom.Configuration(builder.Configuration);
            });
            builder.Services.AddHttpLogging(logging =>
            {
                logging.LoggingFields = HttpLoggingFields.RequestBody | HttpLoggingFields.ResponseBody;
                logging.RequestBodyLogLimit = 4096;
                logging.ResponseBodyLogLimit = 4096;
            });
        }
    }
}

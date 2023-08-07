using Serilog;

namespace GroceryListHelper.Server.Installers;

public class LoggingInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        if (builder.Configuration.GetValue<bool>("AddLogging"))
        {
            builder.Host.UseSerilog((context, services, configuration) =>
            {
                configuration.ReadFrom.Configuration(builder.Configuration);
            });
        }
    }
}

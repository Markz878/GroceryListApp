namespace GroceryListHelper.Server.Installers;

public sealed class ApplicationInsightsInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        if (builder.Environment.IsProduction())
        {
            builder.Services.AddApplicationInsightsTelemetry();
        }
    }
}

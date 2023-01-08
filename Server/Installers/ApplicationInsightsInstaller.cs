namespace GroceryListHelper.Server.Installers;

public sealed class ApplicationInsightsInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddApplicationInsightsTelemetry();
    }
}

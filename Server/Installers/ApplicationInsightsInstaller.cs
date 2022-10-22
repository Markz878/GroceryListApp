namespace GroceryListHelper.Server.Installers;

public class ApplicationInsightsInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddApplicationInsightsTelemetry();
    }
}

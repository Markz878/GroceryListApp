namespace GroceryListHelper.Server.Installers
{
    public class ApplicationInsightsInstaller : IInstaller
    {
        public void Install(IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddApplicationInsightsTelemetry();
        }
    }
}

using GroceryListHelper.Server.HelperMethods;

namespace GroceryListHelper.Server.Installers;

public class TimerMiddlewareInstaller : IInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<TimerMiddleware>();
    }
}

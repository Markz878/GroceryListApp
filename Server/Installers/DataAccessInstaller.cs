using GroceryListHelper.DataAccess.HelperMethods;

namespace GroceryListHelper.Server.Installers;

public class DataAccessInstaller : IInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDataAccessServices(configuration);
    }
}

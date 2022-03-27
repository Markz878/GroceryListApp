using GroceryListHelper.DataAccess.HelperMethods;

namespace GroceryListHelper.Server.Installers;

public class DataAccessInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddDataAccessServices(builder.Configuration);
    }
}

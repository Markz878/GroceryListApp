using GroceryListHelper.Client.HelperMethods;
using GroceryListHelper.Shared.Models.HelperModels;

namespace GroceryListHelper.Server.Installers;

public sealed class DataAccessInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddDataAccessServices();
        builder.Services.AddSingleton<RenderLocation, ServerRenderedLocation>();
        builder.Services.AddMemoryCache();
        builder.Services.AddScoped<AppState>();
    }
}

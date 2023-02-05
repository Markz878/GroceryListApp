using GroceryListHelper.Client.ViewModels;
using GroceryListHelper.Shared.Models.HelperModels;

namespace GroceryListHelper.Server.Installers;

public sealed class DataAccessInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddDataAccessServices(builder.Configuration, builder.Environment.IsDevelopment());
        builder.Services.AddScoped<ICartHubClient, ServerCartHubClient>();
        builder.Services.AddScoped<ICartProductsService, ServerCartProductsServiceProvider>();
        builder.Services.AddScoped<IStoreProductsService, ServerStoreProductsServiceProvider>();
        builder.Services.AddScoped<IndexViewModel>();
        builder.Services.AddScoped<ModalViewModel>();
        builder.Services.AddSingleton<RenderLocation, ServerRenderedLocation>();
    }
}

using GroceryListHelper.Client.HelperMethods;
using GroceryListHelper.Client.ViewModels;
using GroceryListHelper.Server.Services;
using GroceryListHelper.Shared.Interfaces;

namespace GroceryListHelper.Server.Installers;

public class DataAccessInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddDataAccessServices(builder.Configuration, builder.Environment.IsDevelopment());
        builder.Services.AddScoped<ICartHubBuilder, CartHubBuilder>();
        builder.Services.AddScoped<ICartProductsService, HostCartProductsServiceProvider>();
        builder.Services.AddScoped<IStoreProductsService, HostStoreProductsServiceProvider>();
        builder.Services.AddScoped<IndexViewModel>();
        builder.Services.AddScoped<ModalViewModel>();
    }
}

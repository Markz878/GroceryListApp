using GroceryListHelper.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GroceryListHelper.DataAccess.HelperMethods;

public static class ServiceExtensions
{
    public static void AddDataAccessServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<GroceryStoreDbContext>(options =>
            options.UseCosmos(configuration.GetConnectionString("Cosmos"), "GroceryListDb"));
        services.AddScoped<ICartProductRepository, CartProductRepository>();
        services.AddScoped<IStoreProductRepository, StoreProductRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
    }
}

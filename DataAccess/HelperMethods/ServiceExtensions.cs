using GroceryListHelper.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Azure.Identity;

namespace GroceryListHelper.DataAccess.HelperMethods;

public static class ServiceExtensions
{
    public static void AddDataAccessServices(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
    {

        services.AddDbContext<GroceryStoreDbContext>(options =>
        {
            if (isDevelopment)
            {
                options.UseCosmos(configuration.GetConnectionString("Cosmos") ?? throw new ArgumentNullException("CosmosDb connection string"), "GroceryListDb");
                options.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging(true);
            }
            else
            {
                options.UseCosmos(configuration.GetConnectionString("Cosmos") ?? throw new ArgumentNullException("CosmosDb connection string"), new ManagedIdentityCredential(), "GroceryListDb");
            }
        });
        services.AddScoped<ICartProductRepository, CartProductRepository>();
        services.AddScoped<IStoreProductRepository, StoreProductRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
    }
}

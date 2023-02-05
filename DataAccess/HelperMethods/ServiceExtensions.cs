using GroceryListHelper.DataAccess.Repositories;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GroceryListHelper.DataAccess.HelperMethods;

public static class ServiceExtensions
{
    public static void AddDataAccessServices(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
    {
        services.AddDbContext<GroceryStoreDbContext>(options =>
        {
            if (isDevelopment)
            {
                options.UseCosmos(configuration.GetConnectionString("Cosmos") ?? throw new ArgumentNullException("CosmosDb connection string"), "GroceryListDb", x =>
                {
                    x.HttpClientFactory(() =>
                    {
                        HttpMessageHandler httpMessageHandler = new HttpClientHandler()
                        {
                            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                        };
                        return new HttpClient(httpMessageHandler);
                    });
                    x.ConnectionMode(ConnectionMode.Gateway);
                });
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging(true);
            }
            else
            {
                options.UseCosmos(configuration.GetConnectionString("Cosmos") ?? throw new ArgumentNullException("CosmosDb connection string"), "GroceryListDb");
                options.EnableThreadSafetyChecks(false);
            }
        });
        services.AddScoped<ICartProductRepository, CartProductRepository>();
        services.AddScoped<IStoreProductRepository, StoreProductRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
    }
}

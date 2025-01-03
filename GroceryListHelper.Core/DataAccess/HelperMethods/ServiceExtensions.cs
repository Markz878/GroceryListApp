using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace GroceryListHelper.Core.DataAccess.HelperMethods;

public static class ServiceExtensions
{
    public static void AddDataAccessServices(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
    {
        CosmosClientOptions options = new()
        {
            ApplicationName = "GroceryListHelper",
            ConnectionMode = ConnectionMode.Direct,
            UseSystemTextJsonSerializerWithOptions = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            },
            EnableContentResponseOnWrite = false,
        };
        if (isDevelopment)
        {
            services.AddSingleton(new CosmosClient(configuration.GetConnectionString("CosmosDb"), options));
        }
        else
        {
            services.AddSingleton(new CosmosClient(configuration.GetConnectionString("CosmosDb"), new ManagedIdentityCredential(), options));
        }
        services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining<CoreMarker>());
    }
}

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Collections.Concurrent;

namespace GroceryListHelper.Server.Installers;

public class HealthChecksInstaller : IInstaller
{
    public void Install(IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddHealthChecks().AddCheck<DbHealthCheck>("db_health_check", HealthStatus.Unhealthy, new[] { "service", "database" }, TimeSpan.FromSeconds(10));
    }
}

internal class DbHealthCheck : IHealthCheck
{
    private static readonly ConcurrentDictionary<string, CosmosClient> _connections = new();
    private readonly string connectionString;
    private readonly string databaseName = "GroceryListDb";
    private readonly string[] containers = new[] { "Users", "CartProducts", "StoreProducts" };

    public DbHealthCheck(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("Cosmos");
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_connections.TryGetValue(connectionString, out CosmosClient cosmosClient))
            {
                cosmosClient = new CosmosClient(connectionString);
                if (!_connections.TryAdd(connectionString, cosmosClient))
                {
                    cosmosClient.Dispose();
                    cosmosClient = _connections[connectionString];
                }
            }
            await cosmosClient.ReadAccountAsync();
            Database database = cosmosClient.GetDatabase(databaseName);
            await database.ReadAsync(cancellationToken: cancellationToken);
            foreach (string container in containers)
            {
                await database.GetContainer(container).ReadContainerAsync(cancellationToken: cancellationToken);
            }
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Could not connect to cosmos db and find all containers.", ex);
        }
    }
}
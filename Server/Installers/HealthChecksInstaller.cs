namespace GroceryListHelper.Server.Installers;

public sealed class HealthChecksInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks().AddCheck<DbHealthCheck>("db_health_check", HealthStatus.Unhealthy, new[] { "service", "database" }, TimeSpan.FromSeconds(10));
    }
}

internal class DbHealthCheck : IHealthCheck
{
    private static readonly ConcurrentDictionary<string, CosmosClient> _connections = new();
    private readonly string connectionString;
    private readonly string databaseName = "GroceryListDb";
    private readonly string[] containers = new[] { "UserCartGroups", "CartProducts", "StoreProducts" };
    private readonly ILogger<DbHealthCheck> logger;

    public DbHealthCheck(IConfiguration configuration, ILogger<DbHealthCheck> logger)
    {
        connectionString = configuration.GetConnectionString("Cosmos") ?? throw new ArgumentNullException("CosmosDb connection string");
        this.logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Starting health check...");
            if (!_connections.TryGetValue(connectionString, out CosmosClient? cosmosClient))
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
            DatabaseResponse databaseResponse = await database.ReadAsync(cancellationToken: cancellationToken);
            logger.LogInformation("Got database response for database id {id}.", databaseResponse.Database.Id);
            foreach (string container in containers)
            {
                ContainerResponse containerResponse = await database.GetContainer(container).ReadContainerAsync(cancellationToken: cancellationToken);
                logger.LogInformation("Got container {container} response in health check.", container);
            }
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Health check failed.");
            return HealthCheckResult.Unhealthy("Could not connect to cosmos db and find all containers.", ex);
        }
    }
}
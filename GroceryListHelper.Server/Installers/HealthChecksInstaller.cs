using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GroceryListHelper.Server.Installers;

public sealed class HealthChecksInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks().AddCheck<DbHealthCheck>("db_health_check", HealthStatus.Unhealthy, ["service", "database"], TimeSpan.FromSeconds(10));
    }
}

internal class DbHealthCheck(CosmosClient db, ILogger<DbHealthCheck> logger) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            AccountProperties account = await db.ReadAccountAsync();
            //AsyncPageable<TableItem> tablePages = tableServiceClient.QueryAsync(cancellationToken: cancellationToken);
            //await foreach (Page<TableItem> tablePage in tablePages.AsPages())
            //{
            //    IEnumerable<string> existingTableNames = tablePage.Values.Select(x => x.Name);
            //    foreach (string tableName in tableNames)
            //    {
            //        if (!existingTableNames.Contains(tableName))
            //        {
            //            return HealthCheckResult.Unhealthy($"No table called {tableName} found in database.");
            //        }
            //    }
            //}
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Health check failed.");
            return HealthCheckResult.Unhealthy("Could not connect to database.", ex);
        }
    }
}
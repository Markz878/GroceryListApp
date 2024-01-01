using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GroceryListHelper.Server.Installers;

public sealed class HealthChecksInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks().AddCheck<DbHealthCheck>("db_health_check", HealthStatus.Unhealthy, ["service", "database"], TimeSpan.FromSeconds(10));
    }
}

internal class DbHealthCheck(TableServiceClient tableServiceClient, ILogger<DbHealthCheck> logger) : IHealthCheck
{
    private readonly TableServiceClient tableServiceClient = tableServiceClient;
    private readonly ILogger<DbHealthCheck> logger = logger;
    private readonly string[] tableNames = ServiceExtensions.GetTables();

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            AsyncPageable<TableItem> tablePages = tableServiceClient.QueryAsync(cancellationToken: cancellationToken);
            await foreach (Page<TableItem> tablePage in tablePages.AsPages())
            {
                IEnumerable<string> existingTableNames = tablePage.Values.Select(x => x.Name);
                foreach (string tableName in tableNames)
                {
                    if (!existingTableNames.Contains(tableName))
                    {
                        return HealthCheckResult.Unhealthy($"No table called {tableName} found in database.");
                    }
                }
            }
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Health check failed.");
            return HealthCheckResult.Unhealthy("Could not connect to database.", ex);
        }
    }
}
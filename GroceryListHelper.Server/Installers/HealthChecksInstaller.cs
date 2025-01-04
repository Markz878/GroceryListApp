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
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Health check failed.");
            return HealthCheckResult.Unhealthy("Could not connect to database.", ex);
        }
    }
}
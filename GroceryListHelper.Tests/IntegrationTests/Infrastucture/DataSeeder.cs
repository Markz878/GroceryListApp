using GroceryListHelper.Core;
using Microsoft.Azure.Cosmos;

namespace GroceryListHelper.Tests.IntegrationTests.Infrastucture;

internal static class DataSeeder
{
    public static async Task SeedDatabase(this IServiceProvider services)
    {
        CosmosClient client = services.GetRequiredService<CosmosClient>();
        await client.GetDatabase(DataAccessConstants.Database).DeleteStreamAsync();
        Database database = await client.CreateDatabaseAsync(DataAccessConstants.Database);
        await database.CreateContainerIfNotExistsAsync(GetContainerProperties(DataAccessConstants.CartProductsContainer, "/userId", "/unitPrice/?", "/amount/?", "/order/?", "/isCollected/?", "/_ts/?"));
        await database.CreateContainerIfNotExistsAsync(GetContainerProperties(DataAccessConstants.StoreProductsContainer, "/userId", "/unitPrice/?", "/_ts/?"));
        await database.CreateContainerIfNotExistsAsync(GetContainerProperties(DataAccessConstants.UsersContainer, "/id", "/_ts/?"));
        await database.CreateContainerIfNotExistsAsync(GetContainerProperties(DataAccessConstants.DataProtectionKeysContainer, "/id", "/xmlData/?", "/_ts/?"));
        await database.CreateContainerIfNotExistsAsync(GetContainerProperties(DataAccessConstants.CartGroupsContainer, "/id", "/name/?", "/_ts/?"));
    }

    private static ContainerProperties GetContainerProperties(string containerName, string partitionKey, params IEnumerable<string> exludePaths)
    {
        ContainerProperties containerProperties = new(containerName, partitionKey)
        {
            IndexingPolicy = new IndexingPolicy()
            {
                Automatic = true,
                IndexingMode = IndexingMode.Consistent,
                IncludedPaths =
                {
                    new IncludedPath { Path = "/*" }
                }
            }
        };
        foreach (string path in exludePaths)
        {
            containerProperties.IndexingPolicy.ExcludedPaths.Add(new ExcludedPath() { Path = path });
        }
        return containerProperties;
    }
}

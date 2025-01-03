using GroceryListHelper.Core;
using Microsoft.Azure.Cosmos;

namespace GroceryListHelper.Tests.IntegrationTests.Infrastucture;

internal static class DataSeeder
{
    public static async Task SeedDatabase(this IServiceProvider services)
    {
        CosmosClient client = services.GetRequiredService<CosmosClient>();
        Database database = await client.CreateDatabaseIfNotExistsAsync(DataAccessConstants.Database);
        await database.CreateContainerIfNotExistsAsync(DataAccessConstants.CartProductsContainer, "/userId");
        await database.CreateContainerIfNotExistsAsync(DataAccessConstants.StoreProductsContainer, "/userId");
        await database.CreateContainerIfNotExistsAsync(DataAccessConstants.UsersContainer, "/id");
        await database.CreateContainerIfNotExistsAsync(DataAccessConstants.DataProtectionKeysContainer, "/id");
        await database.CreateContainerIfNotExistsAsync(DataAccessConstants.CartGroupsContainer, "/id");
    }
}

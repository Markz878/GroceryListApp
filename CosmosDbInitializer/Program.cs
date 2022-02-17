using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

try
{
    IConfiguration configuration = new ConfigurationBuilder().AddUserSecrets(typeof(Program).Assembly).Build();
    CosmosClient cosmosClient = new(configuration["LocalConnectionString"]);
    QueryDefinition queryDefinition = new("SELECT * FROM c");
    using FeedIterator<DatabaseProperties> feedIterator = cosmosClient.GetDatabaseQueryIterator<DatabaseProperties>(queryDefinition);
    while (feedIterator.HasMoreResults)
    {
        FeedResponse<DatabaseProperties> response = await feedIterator.ReadNextAsync();
        foreach (DatabaseProperties database in response)
        {
            Console.WriteLine(database.Id);
            Database db = cosmosClient.GetDatabase(database.Id);
            await db.DeleteAsync();
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

try
{
    IConfiguration configuration = new ConfigurationBuilder().AddUserSecrets(typeof(Program).Assembly).Build();
    CosmosClient cosmosClient = new(configuration["LocalConnectionString"]);
    Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync("GroceryListDb", throughput: 1000);
    Container userContainer = await database.CreateContainerIfNotExistsAsync("Users", "/id");
    Container cartProductsContainer = await database.CreateContainerIfNotExistsAsync("CartProducts", "/UserId");
    Container storeProductsContainer = await database.CreateContainerIfNotExistsAsync("StoreProducts", "/UserId");
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
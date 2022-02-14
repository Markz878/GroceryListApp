using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

try
{
    IConfiguration configuration = new ConfigurationBuilder().AddUserSecrets(typeof(Program).Assembly).Build();

    CosmosClient cosmosClient = new(configuration["CosmosConnectionString"]);
    Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync("GroceryListDb", throughput: 1000);
    Container userContainer = await database.CreateContainerIfNotExistsAsync("Users", "/id");
    Container cartProductsContainer = await database.CreateContainerIfNotExistsAsync("CartProducts", "/UserId");
    Container storeProductsContainer = await database.CreateContainerIfNotExistsAsync("StoreProducts", "/UserId");
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
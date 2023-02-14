using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

IConfiguration configuration = new ConfigurationBuilder().AddUserSecrets(typeof(Program).Assembly).Build();
CosmosClient cosmosClient = new(configuration["LocalConnectionString"]);
Database database = cosmosClient.GetDatabase("GroceryListDb");
//Container cartProductsContainer = await database.CreateContainerIfNotExistsAsync("CartProducts", "/UserId");
//Container storeProductsContainer = await database.CreateContainerIfNotExistsAsync("StoreProducts", "/UserId");
//Container userCartGroupsContainer = await database.CreateContainerIfNotExistsAsync("CartGroups", "/Id");
Container userCartGroupsContainer = database.GetContainer("CartGroups");

ItemResponse<Test> response = await userCartGroupsContainer.CreateItemAsync(new Test(Guid.NewGuid(), "AAF", new HashSet<Test2>() { new Test2("QQQ"), new Test2("AAA") }));
ItemResponse<Test> response2 = await userCartGroupsContainer.CreateItemAsync(new Test(Guid.NewGuid(), "VVV", new HashSet<Test2>() { new Test2("QQQ"), new Test2("DDD") }));
//db.UserCartGroups.Add(group);
//await db.SaveChangesAsync();
//try
//{
//    IConfiguration configuration = new ConfigurationBuilder().AddUserSecrets(typeof(Program).Assembly).Build();
//    CosmosClient cosmosClient = new(configuration["LocalConnectionString"]);
//    QueryDefinition queryDefinition = new("SELECT * FROM c");
//    using FeedIterator<DatabaseProperties> feedIterator = cosmosClient.GetDatabaseQueryIterator<DatabaseProperties>(queryDefinition);
//    while (feedIterator.HasMoreResults)
//    {
//        FeedResponse<DatabaseProperties> response = await feedIterator.ReadNextAsync();
//        foreach (DatabaseProperties database in response)
//        {
//            Console.WriteLine(database.Id);
//            Database db = cosmosClient.GetDatabase(database.Id);
//            await db.DeleteAsync();
//        }
//    }
//}
//catch (Exception ex)
//{
//    Console.WriteLine(ex.Message);
//}

//try
//{
//    IConfiguration configuration = new ConfigurationBuilder().AddUserSecrets(typeof(Program).Assembly).Build();
//    CosmosClient cosmosClient = new(configuration["LocalConnectionString"]);
//    Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync("GroceryListDb", throughput: 1000);
//    Container cartProductsContainer = await database.CreateContainerIfNotExistsAsync("CartProducts", "/UserId");
//    Container storeProductsContainer = await database.CreateContainerIfNotExistsAsync("StoreProducts", "/UserId");
//    Container userCartGroupsContainer = await database.CreateContainerIfNotExistsAsync("UserCartGroups", "/HostId");
//}
//catch (Exception ex)
//{
//    Console.WriteLine(ex.Message);
//}


public record Test(Guid id, string Name, HashSet<Test2> Test2s);
public record Test2(string Name);
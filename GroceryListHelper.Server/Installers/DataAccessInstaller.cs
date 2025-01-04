using GroceryListHelper.Core;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.Azure.Cosmos;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace GroceryListHelper.Server.Installers;

public sealed class DataAccessInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddDataAccessServices(builder.Configuration, builder.Environment.IsDevelopment());
        builder.Services.AddMemoryCache();
        builder.Services.AddDataProtection().PersistKeysToAzureTableStorage();
    }
}

public static class DataProtectionExtensions
{
    public static IDataProtectionBuilder PersistKeysToAzureTableStorage(this IDataProtectionBuilder builder)
    {
        ServiceProvider serviceProvider = builder.Services.BuildServiceProvider();
        CosmosClient cosmosClient = serviceProvider.GetRequiredService<CosmosClient>();
        ILogger<CosmosStorageRepository> logger = serviceProvider.GetRequiredService<ILogger<CosmosStorageRepository>>();
        builder.Services.Configure<KeyManagementOptions>(options =>
        {
            options.XmlRepository = new CosmosStorageRepository(cosmosClient, logger);
        });
        return builder;
    }
}



public class CosmosStorageRepository(CosmosClient db, ILogger<CosmosStorageRepository> logger) : IXmlRepository
{
    private readonly Container dataKeysContainer = db.GetContainer(DataAccessConstants.Database, DataAccessConstants.DataProtectionKeysContainer);

    public IReadOnlyCollection<XElement> GetAllElements()
    {
        try
        {
            List<XElement> result = Task.Run(async () =>
            {
                return await dataKeysContainer.Query<DataProtectionKeyEntity, XElement>(x => XElement.Parse(x.XmlData));
            }).Result;
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Could not retrieve data protection keys.");
            return [];
        }
    }

    public async void StoreElement(XElement element, string friendlyName)
    {
        try
        {
            DataProtectionKeyEntity dataProtectionEntity = new()
            {
                Id = friendlyName,
                XmlData = element.ToString()
            };
            await dataKeysContainer.UpsertItemAsync(dataProtectionEntity);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Could not store data protection key.");
        }
    }
}

public sealed class DataProtectionKeyEntity
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    public required string XmlData { get; set; }
}

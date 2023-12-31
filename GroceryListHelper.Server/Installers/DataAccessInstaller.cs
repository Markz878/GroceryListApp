using Azure;
using Azure.Data.Tables;
using GroceryListHelper.Shared.Models.HelperModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.Repositories;
using System.Xml.Linq;

namespace GroceryListHelper.Server.Installers;

public sealed class DataAccessInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddDataAccessServices();
        builder.Services.AddMemoryCache();
        builder.Services.AddDataProtection().PersistKeysToAzureTableStorage(builder.Configuration, builder.Environment.IsDevelopment());
    }
}

public static class DataProtectionExtensions
{
    public static IDataProtectionBuilder PersistKeysToAzureTableStorage(this IDataProtectionBuilder builder, IConfiguration configuration, bool isDevelopment)
    {
        builder.Services.Configure<KeyManagementOptions>(options =>
        {
            TableServiceClient tableServiceClient = isDevelopment ?
            new TableServiceClient("DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;") :
            new TableServiceClient(new Uri($"{configuration["TableStorageUri"] ?? throw new ArgumentNullException("TableStorageUri configuration value")}"), new ManagedIdentityCredential());
            options.XmlRepository = new AzureTableStorageRepository(tableServiceClient);
        });
        return builder;
    }
}



public class AzureTableStorageRepository : IXmlRepository
{
    private readonly TableClient _tableClient;
    public AzureTableStorageRepository(TableServiceClient tableServiceClient)
    {
        _tableClient = tableServiceClient.GetTableClient("DataProtectionKeys");
        _tableClient.CreateIfNotExists();
    }

    public IReadOnlyCollection<XElement> GetAllElements()
    {
        List<XElement> elements = [];

        try
        {
            Pageable<DataProtectionKeyEntity> query = _tableClient.Query<DataProtectionKeyEntity>();

            foreach (DataProtectionKeyEntity entity in query)
            {
                elements.Add(XElement.Parse(entity.XmlData));
            }
        }
        catch (RequestFailedException ex)
        {
            Console.WriteLine($"Error retrieving keys from Azure Table Storage: {ex.Message}");
        }

        return elements.AsReadOnly();
    }

    public void StoreElement(XElement element, string friendlyName)
    {
        try
        {
            DataProtectionKeyEntity dataProtectionEntity = new()
            {
                PartitionKey = "DataProtectionKeys",
                RowKey = Guid.NewGuid().ToString(),
                XmlData = element.ToString()
            };

            _tableClient.AddEntity(dataProtectionEntity);
        }
        catch (RequestFailedException ex)
        {
            Console.WriteLine($"Error storing keys in Azure Table Storage: {ex.Message}");
        }
    }
}

public class DataProtectionKeyEntity : ITableEntity
{
    public string PartitionKey { get; set; } = "DataProtectionKeys";
    public required string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    public required string XmlData { get; set; }
}

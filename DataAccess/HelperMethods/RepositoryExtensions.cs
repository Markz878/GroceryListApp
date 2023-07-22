using Azure;
using Azure.Data.Tables;

namespace GroceryListHelper.DataAccess.HelperMethods;
internal static class RepositoryExtensions
{
    internal static async Task<List<T>> GetTableEntitiesByPrimaryKey<T>(this TableServiceClient tableServiceClient, string partitionKey, int pageSize = 20, IEnumerable<string>? select = null) where T : class, ITable
    {
        TableClient tableClient = tableServiceClient.GetTableClient(T.GetTableName());
        return await tableClient.GetTableEntitiesByPrimaryKey<T>(partitionKey, pageSize, select);
    }

    internal static async Task<List<T>> GetTableEntitiesByPrimaryKey<T>(this TableClient tableClient, string partitionKey, int pageSize = 20, IEnumerable<string>? select = null) where T : class, ITable
    {
        AsyncPageable<T> tableQueryPage = tableClient.QueryAsync<T>(x => x.PartitionKey == partitionKey, pageSize, select);
        List<T> result = new();
        string? token = null;
        await foreach (Page<T> page in tableQueryPage.AsPages(token, pageSize))
        {
            token = page.ContinuationToken;
            IReadOnlyList<T> pageResults = page.Values;
            result.AddRange(pageResults);
        }
        return result;
    }
}

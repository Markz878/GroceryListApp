namespace GroceryListHelper.Core.DataAccess.HelperMethods;

public static class RepositoryExtensions
{
    public static async Task<List<T>> Query<T>(this Container container, QueryDefinition? query = null)
    {
        List<T> result = [];
        using FeedIterator<T> iterator = query is null ? container.GetItemQueryIterator<T>() : container.GetItemQueryIterator<T>(query);
        while (iterator.HasMoreResults)
        {
            FeedResponse<T> response = await iterator.ReadNextAsync();
            result.AddRange(response);
        }
        return result;
    }

    public static async Task<List<TResult>> Query<TEntity, TResult>(this Container container, Func<TEntity, TResult> map, QueryDefinition? query = null)
    {
        List<TResult> result = [];
        using FeedIterator<TEntity> iterator = query is null ? container.GetItemQueryIterator<TEntity>() : container.GetItemQueryIterator<TEntity>(query);
        while (iterator.HasMoreResults)
        {
            FeedResponse<TEntity> response = await iterator.ReadNextAsync();
            result.AddRange(response.Select(map));
        }
        return result;
    }

    public static async Task BatchDelete(this Container container, IEnumerable<string> ids, PartitionKey partitionKey)
    {
        if (!ids.Any())
        {
            return;
        }
        TransactionalBatch batch = container.CreateTransactionalBatch(partitionKey);
        foreach (string id in ids)
        {
            batch.DeleteItem(id);
        }
        await batch.ExecuteAsync();
    }
}

namespace GroceryListHelper.Core.DataAccess.Models;

internal sealed record StoreProductDbModel : StoreProduct, ITable
{
    public Guid OwnerId { get; set; }
    public string PartitionKey { get => OwnerId.ToString(); set => OwnerId = Guid.Parse(value); }
    public string RowKey { get => Name; set => Name = value; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public static string GetTableName()
    {
        return "StoreProducts";
    }
}

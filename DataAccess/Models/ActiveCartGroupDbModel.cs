using Azure;
using GroceryListHelper.DataAccess.HelperMethods;

namespace GroceryListHelper.DataAccess.Models;

public record ActiveCartGroupDbModel : ITable
{
    public Guid UserId { get; set; }
    public Guid GroupId { get; set; }
    public string PartitionKey { get => UserId.ToString(); set => UserId = Guid.Parse(value); }
    public string RowKey { get => GroupId.ToString(); set => Guid.Parse(value); }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public static string GetTableName()
    {
        return "ActiveCartGroups";
    }
}

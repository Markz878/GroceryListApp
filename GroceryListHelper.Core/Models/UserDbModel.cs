using Azure;
using GroceryListHelper.Core.HelperMethods;

namespace GroceryListHelper.DataAccess.Models;

internal class UserDbModel : ITable
{
    public required string Email { get; set; }
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string PartitionKey { get => Email.ToString(); set => Email = value; }
    public string RowKey { get => Email.ToString(); set => Email = value; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public static string GetTableName()
    {
        return "Users";
    }
}

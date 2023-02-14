using Azure;
using GroceryListHelper.DataAccess.HelperMethods;

namespace GroceryListHelper.DataAccess.Models;

public sealed record CartGroupUserDbModel : ITable
{
    public Guid GroupId { get; set; }
    public required string Name { get; set; }
    public required string MemberEmail { get; set; }
    public string PartitionKey { get => GroupId.ToString(); set => GroupId = Guid.Parse(value); }
    public string RowKey { get => MemberEmail; set => MemberEmail = value; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public static string GetTableName()
    {
        return "CartGroupUsers";
    }
}

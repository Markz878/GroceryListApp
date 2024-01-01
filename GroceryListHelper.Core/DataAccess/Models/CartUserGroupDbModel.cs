namespace GroceryListHelper.Core.DataAccess.Models;

internal sealed record CartUserGroupDbModel : ITable
{
    public Guid GroupId { get; set; }
    public required string MemberEmail { get; set; }
    public string PartitionKey { get => MemberEmail; set => MemberEmail = value; }
    public string RowKey { get => GroupId.ToString(); set => GroupId = Guid.Parse(value); }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public static string GetTableName()
    {
        return "CartUserGroups";
    }
}

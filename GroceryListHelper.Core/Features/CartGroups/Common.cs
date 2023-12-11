namespace GroceryListHelper.Core.Features.CartGroups;

public static class Common
{
    public static async Task<bool> CheckGroupAccess(TableServiceClient db, Guid groupId, string userEmail, CancellationToken cancellationToken)
    {
        TableClient cartGroupUserTableClient = db.GetTableClient(CartGroupUserDbModel.GetTableName());
        NullableResponse<CartGroupUserDbModel> group = await cartGroupUserTableClient.GetEntityIfExistsAsync<CartGroupUserDbModel>(groupId.ToString(), userEmail, Array.Empty<string>(), cancellationToken);
        return group.HasValue;
    }
}

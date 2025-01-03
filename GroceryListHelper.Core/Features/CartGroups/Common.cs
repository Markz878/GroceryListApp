namespace GroceryListHelper.Core.Features.CartGroups;

public static class Common
{
    public static async Task<bool> CheckGroupAccess(CosmosClient db, Guid groupId, string userEmail, CancellationToken cancellationToken)
    {
        Container cartGroupContainer = db.GetContainer(DataAccessConstants.Database, DataAccessConstants.CartGroupsContainer);
        string sql = "SELECT * FROM c WHERE c.id = @groupId AND ARRAY_CONTAINS(c.memberEmails, @userEmail)";
        QueryDefinition query = new QueryDefinition(sql)
            .WithParameter("@groupId", groupId.ToString())
            .WithParameter("@userEmail", userEmail);
        List<CartGroupEntity> groups = await cartGroupContainer.Query<CartGroupEntity>(query);
        return groups.Count > 0;
    }
}

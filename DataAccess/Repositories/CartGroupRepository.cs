using Azure;
using Azure.Data.Tables;
using GroceryListHelper.DataAccess.Exceptions;
using GroceryListHelper.DataAccess.HelperMethods;
using GroceryListHelper.DataAccess.Models;
using GroceryListHelper.Shared.Models.CartGroups;

namespace GroceryListHelper.DataAccess.Repositories;

public sealed class CartGroupRepository : ICartGroupRepository
{
    private readonly TableServiceClient db;

    public CartGroupRepository(TableServiceClient db)
    {
        this.db = db;
    }

    public async Task<List<CartGroup>> GetCartGroupsForUser(string userEmail)
    {
        List<CartGroup> result = new();
        List<CartUserGroupDbModel> cartUserGroups = await db.GetTableEntitiesByPrimaryKey<CartUserGroupDbModel>(CartUserGroupDbModel.GetTableName(), select: new[] { "GroupId" });
        foreach (CartUserGroupDbModel? cartUserGroup in cartUserGroups.DistinctBy(x=>x.GroupId))
        {
            List<CartGroupUserDbModel> cartGroupUsers = await db.GetTableEntitiesByPrimaryKey<CartGroupUserDbModel>(cartUserGroup.GroupId.ToString());
            if (cartGroupUsers.Count>1)
            {
                CartGroup group = new() { Id = cartGroupUsers[0].GroupId, Name = cartGroupUsers[0].Name };
                foreach (CartGroupUserDbModel? cartGroupUser in cartGroupUsers.Where(x=>x.MemberEmail != userEmail))
                {
                    group.OtherUsers.Add(cartGroupUser.MemberEmail);
                }
                result.Add(group);
            }
        }
        return result;
        //TableClient cartGroupUserClient = db.GetTableClient(CartGroupUserDbModel.GetTableName());
        //AsyncPageable<CartUserGroupDbModel> cartUserGroupPages = cartUserGroupClient.QueryAsync<CartUserGroupDbModel>(x => x.PartitionKey == userEmail, 20, new[] { "GroupId" });
        //List<CartGroup> result = new();
        //string? token = null;
        //await foreach (Page<CartUserGroupDbModel> cartUserGroupPage in cartUserGroupPages.AsPages(token, 20))
        //{
        //    token = cartUserGroupPage.ContinuationToken;
        //    IReadOnlyList<CartUserGroupDbModel> cartUserGroups = cartUserGroupPage.Values;
        //    foreach (CartUserGroupDbModel cartUserGroup in cartUserGroups)
        //    {
        //        AsyncPageable<CartGroupUserDbModel> cartGroupUserPage = cartGroupUserClient.QueryAsync<CartGroupUserDbModel>(x => x.PartitionKey == cartUserGroup.GroupId.ToString(), 20);
        //        CartGroup? group = null;
        //        await foreach (CartGroupUserDbModel cartGroupUser in cartGroupUserPage)
        //        {
        //            group ??= new() { Id = cartGroupUser.GroupId, Name = cartGroupUser.Name };
        //            group.OtherUsers.Add(cartGroupUser.MemberEmail);
        //        }
        //        if (group is not null)
        //        {
        //            group.OtherUsers.Remove(userEmail);
        //            result.Add(group);
        //        }
        //    }
        //}
        //return result;
        //CosmosClient cosmosClient = db.Database.GetCosmosClient();
        //Database database = cosmosClient.GetDatabase("GroceryListDb");
        //Container userCartGroupsContainer = database.GetContainer("CartGroups");
        //QueryDefinition query = new QueryDefinition("SELECT * FROM CartGroups WHERE ARRAY_CONTAINS(CartGroups.Users, @userEmail)")
        //    .WithParameter("@userEmail", userEmail);
        //List<CartUserGroupDbModel> results = new();
        //using (FeedIterator<CartUserGroupDbModel> resultSetIterator = userCartGroupsContainer.GetItemQueryIterator<CartUserGroupDbModel>(
        //    query,
        //    requestOptions: new QueryRequestOptions()
        //    {
        //        MaxItemCount = 20,
        //        EnableScanInQuery = true
        //    }))
        //{
        //    while (resultSetIterator.HasMoreResults)
        //    {
        //        FeedResponse<CartUserGroupDbModel> response = await resultSetIterator.ReadNextAsync();
        //        results.AddRange(response);
        //        if (response.Diagnostics != null)
        //        {
        //            Console.WriteLine($"\nQueryWithSqlParameters Diagnostics: {response.Diagnostics}");
        //        }
        //    }
        //}
        //return results.Select(x => new CartGroup() { Id = x.GroupId, Name = x.Name, OtherUsers = x.Users.Where(x => x != userEmail).ToHashSet() }).ToList();
    }

    public async Task<CartGroup?> GetCartGroup(Guid groupId, string userEmail)
    {
        TableClient cartGroupUserTableClient = db.GetTableClient(CartGroupUserDbModel.GetTableName());
        List<CartGroupUserDbModel> cartGroupUsers = await db.GetTableEntitiesByPrimaryKey<CartGroupUserDbModel>(groupId.ToString());
        if (cartGroupUsers.Count == 0)
        {
            return null;
        }
        CartGroup? group = new() { Id = groupId, Name = cartGroupUsers[0].Name };
        foreach (var member in cartGroupUsers.Where(x => x.Name != userEmail))
        {
            group.OtherUsers.Add(member.Name);
        }
        return group;
    }

    public async Task<bool> CheckGroupAccess(Guid groupId, string userEmail)
    {
        TableClient cartGroupUserTableClient = db.GetTableClient(CartGroupUserDbModel.GetTableName());
        try
        {
            CartGroupUserDbModel group = await cartGroupUserTableClient.GetEntityAsync<CartGroupUserDbModel>(groupId.ToString(), userEmail);
            return true;
        }
        catch (RequestFailedException)
        {
            return false;
        }
    }

    public async Task<Guid> CreateGroup(string name, HashSet<string> userEmails)
    {
        TableClient cartUserGroupTableClient = db.GetTableClient(CartUserGroupDbModel.GetTableName());
        TableClient cartGroupUserTableClient = db.GetTableClient(CartGroupUserDbModel.GetTableName());
        Guid groupId = Guid.NewGuid();
        foreach (string item in userEmails)
        {
            await cartUserGroupTableClient.AddEntityAsync(new CartUserGroupDbModel()
            {
                MemberEmail = item,
                GroupId = groupId
            });
            await cartGroupUserTableClient.AddEntityAsync(new CartGroupUserDbModel()
            {
                GroupId = groupId,
                MemberEmail = item,
                Name = name
            });
        }
        return groupId;
    }

    public async Task DeleteCartGroup(Guid groupId, string userEmail)
    {
        List<CartGroupUserDbModel> cartGroupUsers = await DeleteCartGroupUsers(groupId);
        await DeleteCartUserGroups(cartGroupUsers);
        await DeleteCartGroupProducts(groupId);
    }

    private async Task DeleteCartGroupProducts(Guid groupId)
    {
        TableClient cartProductsTableClient = db.GetTableClient(CartProductDbModel.GetTableName());
        List<CartProductDbModel> cartProducts = await cartProductsTableClient.GetTableEntitiesByPrimaryKey<CartProductDbModel>(groupId.ToString());
        Response<IReadOnlyList<Response>> response3 = await cartProductsTableClient.SubmitTransactionAsync(cartProducts.Select(x => new TableTransactionAction(TableTransactionActionType.Delete, x)));
    }

    private async Task DeleteCartUserGroups(List<CartGroupUserDbModel> cartGroupUsers)
    {
        TableClient cartUserGroupTableClient = db.GetTableClient(CartUserGroupDbModel.GetTableName());
        Response<IReadOnlyList<Response>> response2 = await cartUserGroupTableClient.SubmitTransactionAsync(cartGroupUsers.Select(x => new TableTransactionAction(TableTransactionActionType.Delete, new CartUserGroupDbModel() { MemberEmail = x.MemberEmail, GroupId = x.GroupId })));
    }

    private async Task<List<CartGroupUserDbModel>> DeleteCartGroupUsers(Guid groupId)
    {
        TableClient cartGroupUserTableClient = db.GetTableClient(CartGroupUserDbModel.GetTableName());
        List<CartGroupUserDbModel> cartGroupUsers = await cartGroupUserTableClient.GetTableEntitiesByPrimaryKey<CartGroupUserDbModel>(groupId.ToString());
        Response<IReadOnlyList<Response>> response1 = await cartGroupUserTableClient.SubmitTransactionAsync(cartGroupUsers.Select(x => new TableTransactionAction(TableTransactionActionType.Delete, x)));
        return cartGroupUsers;
    }

    public async Task UserJoinedSharing(Guid userId, Guid groupId)
    {
        TableClient activeCartGroupTableClient = db.GetTableClient(ActiveCartGroupDbModel.GetTableName());
        await activeCartGroupTableClient.AddEntityAsync(new ActiveCartGroupDbModel()
        {
            GroupId = groupId,
            UserId = userId
        });
    }

    public async Task UserLeftSharing(Guid userId, Guid groupId)
    {
        TableClient activeCartGroupTableClient = db.GetTableClient(ActiveCartGroupDbModel.GetTableName());
        await activeCartGroupTableClient.DeleteEntityAsync(userId.ToString(), groupId.ToString());
    }

    public async Task<Guid?> GetUserCurrentShareGroup(Guid userId)
    {
        TableClient activeCartGroupTableClient = db.GetTableClient(ActiveCartGroupDbModel.GetTableName());
        AsyncPageable<ActiveCartGroupDbModel> activeCartGroupPages = activeCartGroupTableClient.QueryAsync<ActiveCartGroupDbModel>(x => x.UserId == userId);
        await foreach (Page<ActiveCartGroupDbModel> activeCartGroupPage in activeCartGroupPages.AsPages())
        {
            foreach (ActiveCartGroupDbModel activeCartGroup in activeCartGroupPage.Values)
            {
                return activeCartGroup.GroupId;
            }
        }
        return null;
    }

    public async Task<Exception?> UpdateGroupName(Guid groupId, string newName)
    {
        TableClient cartGroupUserTableClient = db.GetTableClient(CartGroupUserDbModel.GetTableName());
        List<CartGroupUserDbModel> cartGroupUsers = await cartGroupUserTableClient.GetTableEntitiesByPrimaryKey<CartGroupUserDbModel>(groupId.ToString());
        if (cartGroupUsers.Count == 0)
        {
            return NotFoundException.ForType<CartGroupUserDbModel>();
        }
        foreach (var cartGroupUser in cartGroupUsers)
        {
            cartGroupUser.Name = newName;
        }
        Response<IReadOnlyList<Response>> response = await cartGroupUserTableClient.SubmitTransactionAsync(cartGroupUsers.Select(x => new TableTransactionAction(TableTransactionActionType.UpdateMerge, x)));
        return null;
    }

}

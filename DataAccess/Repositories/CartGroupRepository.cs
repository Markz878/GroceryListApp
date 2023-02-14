using Azure;
using Azure.Data.Tables;
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
        TableClient tableClient = db.GetTableClient(CartUserGroupDbModel.GetTableName());
        AsyncPageable<CartUserGroupDbModel> cartUserGroupPages = tableClient.QueryAsync<CartUserGroupDbModel>(x => x.PartitionKey == userEmail, 20);
        List<CartGroup> result = new();
        string? token = null;
        await foreach (Page<CartUserGroupDbModel> cartUserGroupPage in cartUserGroupPages.AsPages(token, 20))
        {
            token = cartUserGroupPage.ContinuationToken;
            IReadOnlyList<CartUserGroupDbModel> cartUserGroups = cartUserGroupPage.Values;
            foreach (CartUserGroupDbModel cartUserGroup in cartUserGroups)
            {
                AsyncPageable<CartGroupUserDbModel> cartGroupUserPage = tableClient.QueryAsync<CartGroupUserDbModel>(x => x.PartitionKey == cartUserGroup.GroupId.ToString(), 20);
                CartGroup? group = null;
                await foreach (CartGroupUserDbModel cartGroupUser in cartGroupUserPage)
                {
                    group ??= new() { Id = cartGroupUser.GroupId, Name = cartGroupUser.Name };
                    group.OtherUsers.Add(cartGroupUser.MemberEmail);
                }
                if (group is not null)
                {
                    group.OtherUsers.Remove(userEmail);
                    result.Add(group);
                }
            }
        }
        return result;
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
        TableClient cartUserGroupTableClient = db.GetTableClient(CartUserGroupDbModel.GetTableName());
        TableClient cartGroupUserTableClient = db.GetTableClient(CartGroupUserDbModel.GetTableName());
        AsyncPageable<CartUserGroupDbModel> cartUserGroupPages = cartUserGroupTableClient.QueryAsync<CartUserGroupDbModel>(x => x.PartitionKey == groupId.ToString(), 20);
        string? token = null;
        CartGroup? result = null;
        await foreach (Page<CartUserGroupDbModel> cartUserGroupPage in cartUserGroupPages.AsPages(token, 20))
        {
            token = cartUserGroupPage.ContinuationToken;
            IReadOnlyList<CartUserGroupDbModel> cartUserGroups = cartUserGroupPage.Values;
            foreach (CartUserGroupDbModel cartUserGroup in cartUserGroups)
            {
                AsyncPageable<CartGroupUserDbModel> cartGroupUserPage = cartGroupUserTableClient.QueryAsync<CartGroupUserDbModel>(x => x.PartitionKey == cartUserGroup.GroupId.ToString(), 20);
                CartGroup? group = null;
                await foreach (CartGroupUserDbModel cartGroupUser in cartGroupUserPage)
                {
                    group ??= new() { Id = cartGroupUser.GroupId, Name = cartGroupUser.Name };
                    group.OtherUsers.Add(cartGroupUser.MemberEmail);
                }
                group?.OtherUsers.Remove(userEmail);
            }
        }
        return result;
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

    public async Task RemoveUserFromCartGroup(Guid groupId, string userEmail)
    {
        TableClient cartUserGroupTableClient = db.GetTableClient(CartUserGroupDbModel.GetTableName());
        TableClient cartGroupUserTableClient = db.GetTableClient(CartGroupUserDbModel.GetTableName());
        await cartUserGroupTableClient.DeleteEntityAsync(userEmail, groupId.ToString());
        await cartGroupUserTableClient.DeleteEntityAsync(groupId.ToString(), userEmail);
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
}

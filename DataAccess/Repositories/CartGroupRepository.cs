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
        List<CartUserGroupDbModel> cartUserGroups = await db.GetTableEntitiesByPrimaryKey<CartUserGroupDbModel>(userEmail, select: new[] { "GroupId" });
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
    }

    public async Task<CartGroup?> GetCartGroup(Guid groupId, string userEmail)
    {
        TableClient cartGroupUserTableClient = db.GetTableClient(CartGroupUserDbModel.GetTableName());
        List<CartGroupUserDbModel> cartGroupUsers = await db.GetTableEntitiesByPrimaryKey<CartGroupUserDbModel>(groupId.ToString());
        if (cartGroupUsers.Count == 0)
        {
            return null;
        }
        CartGroup group = new() { Id = groupId, Name = cartGroupUsers[0].Name };
        foreach (var member in cartGroupUsers.Where(x => x.MemberEmail != userEmail))
        {
            group.OtherUsers.Add(member.MemberEmail);
        }
        return group;
    }

    public async Task<bool> CheckGroupAccess(Guid groupId, string userEmail)
    {
        TableClient cartGroupUserTableClient = db.GetTableClient(CartGroupUserDbModel.GetTableName());
        try
        {
            CartGroupUserDbModel group = await cartGroupUserTableClient.GetEntityAsync<CartGroupUserDbModel>(groupId.ToString(), userEmail, Array.Empty<string>());
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
        Guid groupId = await GetNewGroupId(cartGroupUserTableClient);
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

        static async Task<Guid> GetNewGroupId(TableClient cartGroupUserTableClient)
        {
            Guid groupId;
            List<CartGroupUserDbModel> entities;
            do
            {
                groupId = Guid.NewGuid();
                entities = await cartGroupUserTableClient.GetTableEntitiesByPrimaryKey<CartGroupUserDbModel>(groupId.ToString());
            } while (entities.Count > 0);
            return groupId;
        }
    }



    public async Task<Exception?> DeleteCartGroup(Guid groupId, string userEmail)
    {
        bool hasAccess = await CheckGroupAccess(groupId, userEmail);
        if (hasAccess is false)
        {
            return new NotFoundException("Cart group user");
        }
        List<CartGroupUserDbModel> cartGroupUsers = await DeleteCartGroupUsers(groupId);
        await DeleteCartUserGroups(cartGroupUsers);
        await DeleteCartGroupProducts(groupId);
        return null;

        async Task<List<CartGroupUserDbModel>> DeleteCartGroupUsers(Guid groupId)
        {
            TableClient cartGroupUserTableClient = db.GetTableClient(CartGroupUserDbModel.GetTableName());
            List<CartGroupUserDbModel> cartGroupUsers = await cartGroupUserTableClient.GetTableEntitiesByPrimaryKey<CartGroupUserDbModel>(groupId.ToString());
            await cartGroupUserTableClient.SubmitTransactionAsync(cartGroupUsers.Select(x => new TableTransactionAction(TableTransactionActionType.Delete, x)));
            return cartGroupUsers;
        }

        async Task DeleteCartUserGroups(List<CartGroupUserDbModel> cartGroupUsers)
        {
            TableClient cartUserGroupTableClient = db.GetTableClient(CartUserGroupDbModel.GetTableName());
            await cartUserGroupTableClient.SubmitTransactionAsync(cartGroupUsers.Select(x => new TableTransactionAction(TableTransactionActionType.Delete, new CartUserGroupDbModel() { MemberEmail = x.MemberEmail, GroupId = x.GroupId })));
        }

        async Task DeleteCartGroupProducts(Guid groupId)
        {
            TableClient cartProductsTableClient = db.GetTableClient(CartProductDbModel.GetTableName());
            List<CartProductDbModel> cartProducts = await cartProductsTableClient.GetTableEntitiesByPrimaryKey<CartProductDbModel>(groupId.ToString());
            if (cartProducts.Count > 0)
            {
                await cartProductsTableClient.SubmitTransactionAsync(cartProducts.Select(x => new TableTransactionAction(TableTransactionActionType.Delete, x)));
            }
        }
    }

    public async Task UserJoinedSharing(Guid userId, Guid groupId)
    {
        TableClient activeCartGroupTableClient = db.GetTableClient(ActiveCartGroupDbModel.GetTableName());
        try
        {
            await activeCartGroupTableClient.AddEntityAsync(new ActiveCartGroupDbModel()
            {
                GroupId = groupId,
                UserId = userId
            });
        }
        catch (RequestFailedException ex) when (ex.Message.Contains("409"))
        {
        }
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

    public async Task<Exception?> UpdateGroupName(Guid groupId, string newName, string userEmail)
    {
        bool hasAccess = await CheckGroupAccess(groupId, userEmail);
        if (hasAccess is false)
        {
            return new NotFoundException("Cart group user");
        }
        TableClient cartGroupUserTableClient = db.GetTableClient(CartGroupUserDbModel.GetTableName());
        List<CartGroupUserDbModel> cartGroupUsers = await cartGroupUserTableClient.GetTableEntitiesByPrimaryKey<CartGroupUserDbModel>(groupId.ToString());
        foreach (var cartGroupUser in cartGroupUsers)
        {
            cartGroupUser.Name = newName;
        }
        await cartGroupUserTableClient.SubmitTransactionAsync(cartGroupUsers.Select(x => new TableTransactionAction(TableTransactionActionType.UpdateMerge, x)));
        return null;
    }

}

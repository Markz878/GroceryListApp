using GroceryListHelper.DataAccess.Exceptions;
using GroceryListHelper.Shared.Models.HelperModels;
using Microsoft.AspNetCore.SignalR;

namespace GroceryListHelper.Server.Hubs;

[Authorize]
public sealed class CartHub : Hub<ICartHubNotifications>, ICartHubClientActions
{
    private readonly ICartProductRepository db;
    private readonly ICartGroupRepository userRepository;
    private readonly ILogger<CartHub> logger;

    public CartHub(ICartProductRepository productRepository, ICartGroupRepository userRepository, ILogger<CartHub> logger)
    {
        this.db = productRepository;
        this.userRepository = userRepository;
        this.logger = logger;
    }

    public async Task<HubResponse> JoinGroup(Guid groupId)
    {
        Response<string, NotFoundError> getGroupResponse = await userRepository.GetCartGroupName(groupId, GetUserEmail());
        return await getGroupResponse.MatchAsync(async x =>
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToString());
            await userRepository.UserJoinedSharing(GetUserId(), groupId);
            await Clients.Caller.ReceiveCart(await db.GetCartProducts(groupId));
            await Clients.OthersInGroup(groupId.ToString()).GetMessage($"{GetUserEmail()} has joined group cart.");
            return new HubResponse() { SuccessMessage = $"You have joined cart '{x}'" };
        },
        e => Task.FromResult(new HubResponse() { ErrorMessage = "User is not part of a group with given the id." }));
    }

    public async Task<HubResponse> LeaveGroup(Guid groupId)
    {
        Response<string, NotFoundError> getGroupResponse = await userRepository.GetCartGroupName(groupId, GetUserEmail());
        return await getGroupResponse.MatchAsync(async x =>
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId.ToString());
            await userRepository.UserLeftSharing(GetUserId(), groupId);
            await Clients.OthersInGroup(groupId.ToString()).GetMessage($"{GetUserEmail()} has left sharing.");
            return new HubResponse() { SuccessMessage = $"You left '{x}' cart sharing." };
        },
e => Task.FromResult(new HubResponse() { ErrorMessage = "User is not part of a group with given the id." }));
    }

    public async Task<HubResponse> CartItemAdded(CartProduct product)
    {
        try
        {
            Guid groupId = await GetGroupId(GetUserId());
            await db.AddCartProduct(product, groupId);
            CartProductCollectable cartProduct = new()
            {
                Amount = product.Amount,
                Name = product.Name,
                Order = product.Order,
                UnitPrice = product.UnitPrice
            };
            await Clients.OthersInGroup(groupId.ToString()).ItemAdded(cartProduct);
            return new HubResponse() { SuccessMessage = "Saved" };
        }
        catch (Exception ex)
        {
            return new HubResponse() { ErrorMessage = ex.Message };
        }
    }

    public async Task<HubResponse> CartItemModified(CartProductCollectable product)
    {
        try
        {
            Guid groupId = await GetGroupId(GetUserId());
            await db.UpdateProduct(groupId, product);
            await Clients.OthersInGroup(groupId.ToString()).ItemModified(product);
            return new HubResponse() { SuccessMessage = "Updated product in group's cart." };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating item in cart hub with name {name}, host id {hostId} and user email {userId}", product.Name, GetGroupId(GetUserId()), GetUserEmail());
            return new HubResponse() { ErrorMessage = ex.Message };
        }
    }

    public async Task<HubResponse> CartItemDeleted(string name)
    {
        try
        {
            Guid groupId = await GetGroupId(GetUserId());
            await db.DeleteProduct(name, groupId);
            await Clients.OthersInGroup(groupId.ToString()).ItemDeleted(name);
            return new HubResponse();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting item from cart hub with name {name}, host id {hostId} and user email {userId}", name, GetGroupId(GetUserId()), GetUserEmail());
            return new HubResponse() { ErrorMessage = ex.Message };
        }
    }

    private Guid GetUserId()
    {
        string? stringId = Context.User?.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
        Guid id = Guid.Parse(stringId?.Trim('"') ?? "");
        return id;
    }

    private string GetUserEmail()
    {
        string? email = Context.User?.FindFirst("preferred_username")?.Value;
        ArgumentNullException.ThrowIfNull(email);
        return email;
    }

    private async Task<Guid> GetGroupId(Guid userId)
    {
        Guid? groupId = await userRepository.GetUserCurrentShareGroup(userId);
        ArgumentNullException.ThrowIfNull(groupId);
        return groupId.GetValueOrDefault();
    }
}

using GroceryListHelper.Core.Features.CartGroups;
using GroceryListHelper.Shared.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace GroceryListHelper.Server.Hubs;

[Authorize]
public sealed class CartHub(IMediator mediator) : Hub<ICartHubNotifications>, ICartHubClient
{
    public string GetConnectionId()
    {
        return Context.ConnectionId;
    }

    public async Task JoinGroup(Guid groupId)
    {
        await CheckAccess(groupId);
        await Groups.AddToGroupAsync(GetConnectionId(), groupId.ToString());
        await Clients.OthersInGroup(groupId.ToString()).GetMessage($"{GetUserEmail()} has joined cart sharing.");
    }

    public async Task LeaveGroup(Guid groupId)
    {
        await CheckAccess(groupId);
        await Groups.RemoveFromGroupAsync(GetConnectionId(), groupId.ToString());
        await Clients.OthersInGroup(groupId.ToString()).GetMessage($"{GetUserEmail()} has left cart sharing.");
    }

    //public async Task<HubResponse> AddCartProduct(Guid groupId, CartProduct product)
    //{
    //    try
    //    {
    //        await CheckAccess(groupId);
    //        ConflictError? itemExistsError = await mediator.Send(new AddCartProductCommand() { UserId = groupId, CartProduct = product });
    //        if (itemExistsError is null)
    //        {
    //            CartProductCollectable cartProduct = new()
    //            {
    //                Amount = product.Amount,
    //                Name = product.Name,
    //                Order = product.Order,
    //                UnitPrice = product.UnitPrice
    //            };
    //            await Clients.OthersInGroup(groupId.ToString()).ProductAdded(cartProduct);
    //            return new HubResponse();
    //        }
    //        else
    //        {
    //            return new HubResponse() { ErrorMessage = "Product already exists." };
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        return new HubResponse() { ErrorMessage = ex.Message };
    //    }
    //}

    //public async Task<HubResponse> UpdateProduct(Guid groupId, CartProductCollectable product)
    //{
    //    try
    //    {
    //        await CheckAccess(groupId);
    //        NotFoundError? notFoundError = await mediator.Send(new UpdateProductCommand() { UserId = groupId, CartProduct = product });
    //        if (notFoundError is null)
    //        {
    //            await Clients.OthersInGroup(groupId.ToString()).ProductModified(product);
    //        }
    //        return new HubResponse();
    //    }
    //    catch (Exception ex)
    //    {
    //        return new HubResponse() { ErrorMessage = ex.Message };
    //    }
    //}

    //public async Task<HubResponse> DeleteProduct(Guid groupId, string name)
    //{
    //    try
    //    {
    //        await CheckAccess(groupId);
    //        NotFoundError? notFoundError = await mediator.Send(new DeleteCartProductCommand() { UserId = groupId, ProductName = name });
    //        if (notFoundError is null)
    //        {
    //            await Clients.OthersInGroup(groupId.ToString()).ProductDeleted(name);
    //        }
    //        return new HubResponse();
    //    }
    //    catch (Exception ex)
    //    {
    //        logger.LogError(ex, "Error deleting item from cart hub with name {name}, group id {groupId} and user email {userEmail}", name, groupId, GetUserEmail());
    //        return new HubResponse() { ErrorMessage = ex.Message };
    //    }
    //}

    //public async Task<HubResponse> SortProducts(Guid groupId, ListSortDirection sortDirection)
    //{
    //    try
    //    {
    //        await CheckAccess(groupId);
    //        await mediator.Send(new SortCartProductsCommand() { UserId = groupId, SortDirection = sortDirection });
    //        await Clients.OthersInGroup(groupId.ToString()).ProductsSorted(sortDirection);
    //        return new HubResponse();
    //    }
    //    catch (Exception ex)
    //    {
    //        return new HubResponse() { ErrorMessage = ex.Message };
    //    }
    //}

    //public async Task<HubResponse> DeleteProducts(Guid groupId)
    //{
    //    try
    //    {
    //        await CheckAccess(groupId);
    //        await mediator.Send(new ClearCartProductsCommand() { UserId = groupId });
    //        await Clients.OthersInGroup(groupId.ToString()).ProductsDeleted();
    //        return new HubResponse();
    //    }
    //    catch (Exception ex)
    //    {
    //        return new HubResponse() { ErrorMessage = ex.Message };
    //    }
    //}

    private async Task CheckAccess(Guid groupId)
    {
        if (await mediator.Send(new CheckGroupAccessQuery() { GroupId = groupId, UserEmail = GetUserEmail() }) is false)
        {
            throw new UnauthorizedAccessException("User is not part of the group");
        }
    }

    private string GetUserEmail()
    {
        string? email = Context.User?.FindFirst(AuthenticationConstants.EmailClaimName)?.Value;
        ArgumentNullException.ThrowIfNull(email);
        return email;
    }
}

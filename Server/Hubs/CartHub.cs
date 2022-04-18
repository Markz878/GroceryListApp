using GroceryListHelper.DataAccess.Repositories;
using GroceryListHelper.Shared.Interfaces;
using GroceryListHelper.Shared.Models.BaseModels;
using GroceryListHelper.Shared.Models.CartProduct;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace GroceryListHelper.Server.Hubs;

[Authorize]
public class CartHub : Hub<ICartHubNotifications>, ICartHubActions
{
    private readonly ICartProductRepository db;
    private readonly IUserRepository userRepository;
    private readonly ILogger<CartHub> logger;

    public CartHub(ICartProductRepository db, IUserRepository userRepository, ILogger<CartHub> logger)
    {
        this.db = db;
        this.userRepository = userRepository;
        this.logger = logger;
    }

    public async Task<HubResponse> CreateGroup(List<string> allowedUserEmails)
    {
        if (allowedUserEmails.Count == 0)
        {
            return new HubResponse() { ErrorMessage = "There are no allowed users for your cart" };
        }
        string hostId = GetUserId(Context);
        string hostEmail = GetUserEmail(Context);
        string otherUsers = string.Join(", ", allowedUserEmails);
        await Groups.AddToGroupAsync(Context.ConnectionId, hostId);
        allowedUserEmails.Add(hostEmail);
        await userRepository.CreateGroupAllowedEmails(hostId, hostEmail, allowedUserEmails);
        return new HubResponse() { SuccessMessage = $"You are sharing your cart to {otherUsers}." };
    }

    public async Task<HubResponse> JoinGroup(string hostEmail)
    {
        string hostId = await userRepository.GetHostIdFromHostEmail(hostEmail);
        if (hostId == null)
        {
            return new HubResponse() { ErrorMessage = "No user with that email." };
        }
        List<string> emails = await userRepository.GetCartHostAllowedEmails(hostId);
        if (emails.Any())
        {
            if (emails.Contains(GetUserEmail(Context)))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, hostId);
                await Clients.Caller.ReceiveCart(await db.GetCartProductsForUser(hostId));
            }
            else
            {
                return new HubResponse() { ErrorMessage = "You are not allowed to join this cart." };
            }
            await Clients.OthersInGroup(hostId).GetMessage($"{GetUserEmail(Context)} has joined {hostEmail}'s cart.");
            return new HubResponse() { SuccessMessage = $"You have joined {hostEmail}'s cart" };
        }
        else
        {
            return new HubResponse() { ErrorMessage = "There is no cart hosted by that user." };
        }
    }

    public async Task<HubResponse> CartItemAdded(CartProduct product)
    {
        try
        {
            string hostId = await GetHostId();
            Guid id = await db.AddCartProduct(product, hostId);
            CartProductCollectable cartProduct = new()
            {
                Id = id,
                Amount = product.Amount,
                Name = product.Name,
                Order = product.Order,
                UnitPrice = product.UnitPrice
            };
            await Clients.OthersInGroup(hostId.ToString()).ItemAdded(cartProduct);
            return new HubResponse() { SuccessMessage = id.ToString() };
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
            string hostId = await GetHostId();
            await db.UpdateProduct(hostId, product);
            await Clients.OthersInGroup(hostId.ToString()).ItemModified(product);
            return new HubResponse() { SuccessMessage = "Updated product in group's cart." };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating item in cart hub with id {id}, host id {hostId} and user email {userId}", product.Id, await GetHostId(), GetUserEmail(Context));
            return new HubResponse() { ErrorMessage = ex.Message };
        }
    }

    public async Task<HubResponse> CartItemDeleted(Guid id)
    {
        try
        {
            string hostId = await GetHostId();
            await db.DeleteProduct(id, hostId);
            await Clients.OthersInGroup(hostId.ToString()).ItemDeleted(id);
            return new HubResponse();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting item from cart hub with id {id}, host id {hostId} and user email {userId}", id, await GetHostId(), GetUserEmail(Context));
            return new HubResponse() { ErrorMessage = ex.Message };
        }
    }

    public async Task<HubResponse> LeaveGroup()
    {
        string hostId = await GetHostId();
        if (hostId != string.Empty)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, hostId.ToString());
            if (hostId == GetUserId(Context))
            {
                await Clients.OthersInGroup(hostId.ToString()).LeaveCart(GetUserEmail(Context));
                await userRepository.RemoveCartGroup(hostId);
            }
            else
            {
                await Clients.OthersInGroup(hostId.ToString()).GetMessage($"{GetUserEmail(Context)} has left the group.");
            }
            return new HubResponse() { SuccessMessage = "You have left the group." };
        }
        else
        {
            return new HubResponse() { ErrorMessage = "You are not part of any shopping cart." };
        }
    }

    private static string GetUserId(HubCallerContext context)
    {
        string id = context.User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
        return id;
    }

    private static string GetUserEmail(HubCallerContext context)
    {
        string email = context.User.FindFirst("preferred_username").Value;
        return email;
    }

    private async Task<string> GetHostId()
    {
        string email = GetUserEmail(Context);
        string hostId = await userRepository.GetUsersCartHostId(email);
        return hostId;
    }
}

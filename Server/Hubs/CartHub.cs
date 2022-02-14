using GroceryListHelper.DataAccess.Models;
using GroceryListHelper.DataAccess.Repositories;
using GroceryListHelper.Shared;
using GroceryListHelper.Shared.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace GroceryListHelper.Server.Hubs;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class CartHub : Hub<ICartHubNotifications>, ICartHubActions
{
    private readonly ICartProductRepository db;
    private readonly IUserRepository userRepository;
    private readonly ICartHubService cartHubService;

    public CartHub(ICartProductRepository db, IUserRepository userRepository, ICartHubService cartHubService)
    {
        this.db = db;
        this.userRepository = userRepository;
        this.cartHubService = cartHubService;
    }

    public async Task<HubResponse> CreateGroup(List<string> allowedUsers)
    {
        if (allowedUsers.Count == 0)
        {
            return new HubResponse() { ErrorMessage = "There are no allowed users for your cart" };
        }
        await Groups.AddToGroupAsync(Context.ConnectionId, GetUserId(Context).ToString());
        string otherUsers = string.Join(", ", allowedUsers);
        allowedUsers.Add(GetUserEmail(Context));
        cartHubService.GroupAllowedEmails[GetUserId(Context)] = allowedUsers;
        return new HubResponse() { SuccessMessage = $"You are sharing your cart to {otherUsers}." };
    }

    public async Task<HubResponse> JoinGroup(string hostEmail)
    {
        UserDbModel user = await userRepository.GetUserFromEmail(hostEmail);
        if (user == null)
        {
            return new HubResponse() { ErrorMessage = "No user with that email." };
        }
        if (cartHubService.GroupAllowedEmails.TryGetValue(user.Id, out List<string> emails))
        {
            if (emails.Contains(GetUserEmail(Context)))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, user.Id.ToString());
                await Clients.Caller.ReceiveCart((await db.GetCartProductsForUser(user.Id)).ToList());
            }
            else
            {
                return new HubResponse() { ErrorMessage = "You are not allowed to join this cart." };
            }
            await Clients.OthersInGroup(user.Id.ToString()).GetMessage($"{GetUserEmail(Context)} has joined {hostEmail}'s cart.");
            return new HubResponse() { SuccessMessage = $"You have joined {hostEmail}'s cart" };
        }
        else
        {
            return new HubResponse() { ErrorMessage = "There is no cart hosted by that user." };
        }
    }

    private string GetHostId()
    {
        string userEmail = GetUserEmail(Context);
        string hostId = cartHubService.GroupAllowedEmails.FirstOrDefault(x => x.Value.Contains(userEmail)).Key;
        return hostId;
    }

    public async Task<string> CartItemAdded(CartProductCollectable product)
    {
        string hostId = GetHostId();
        product.Id = await db.AddCartProduct(product, hostId);
        await Clients.OthersInGroup(hostId.ToString()).ItemAdded(product);
        return product.Id;
    }

    public async Task CartItemModified(CartProductCollectable product)
    {
        string hostId = GetHostId();
        await db.UpdateProduct(hostId, product);
        await Clients.OthersInGroup(hostId.ToString()).ItemModified(product);
    }

    public async Task CartItemDeleted(string id)
    {
        string hostId = GetHostId();
        await Clients.OthersInGroup(hostId.ToString()).ItemDeleted(id);
        await db.DeleteProduct(id, hostId);
    }

    public async Task<HubResponse> LeaveGroup()
    {
        string hostId = GetHostId();
        if (hostId != string.Empty)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, hostId.ToString());
            if (hostId == GetUserId(Context))
            {
                await Clients.OthersInGroup(hostId.ToString()).LeaveCart(GetUserEmail(Context));
                cartHubService.GroupAllowedEmails.Remove(hostId);
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
        return context.User.FindFirst("id").Value;
    }

    private static string GetUserEmail(HubCallerContext context)
    {
        return context.User.FindFirst(ClaimTypes.Email).Value;
    }
}

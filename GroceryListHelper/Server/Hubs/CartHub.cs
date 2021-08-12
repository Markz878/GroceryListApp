using GroceryListHelper.DataAccess.Models;
using GroceryListHelper.DataAccess.Repositories;
using GroceryListHelper.Shared;
using GroceryListHelper.Shared.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GroceryListHelper.Server.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CartHub : Hub<ICartHubClient>, ICartHub
    {
        private readonly ICartProductRepository db;
        private readonly IUserRepository userRepository;
        private readonly CartHubService cartHubService;

        public CartHub(ICartProductRepository db, IUserRepository userRepository, CartHubService cartHubService)
        {
            this.db = db;
            this.userRepository = userRepository;
            this.cartHubService = cartHubService;
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
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
                    await Clients.Caller.GetCart((await db.GetCartProductsForUser(user.Id)).ToList());
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

        private int GetHostId()
        {
            string userEmail = GetUserEmail(Context);
            int hostId = cartHubService.GroupAllowedEmails.FirstOrDefault(x => x.Value.Contains(userEmail)).Key;
            return hostId;
        }

        public async Task<int> CartItemAdded(CartProductCollectable product)
        {
            int hostId = GetHostId();
            product.Id = await db.AddCartProduct(product, hostId);
            await Clients.OthersInGroup(hostId.ToString()).ItemAdded(product);
            return product.Id;
        }

        public async Task CartItemModified(CartProductCollectable product)
        {
            int hostId = GetHostId();
            await db.UpdateProduct(product.Id, hostId, product);
            await Clients.OthersInGroup(hostId.ToString()).ItemModified(product);
        }

        public async Task CartItemCollected(int id)
        {
            int hostId = GetHostId();
            await Clients.OthersInGroup(hostId.ToString()).ItemCollected(id);
            await db.MarkAsCollected(id, hostId);
        }

        public async Task CartItemDeleted(int id)
        {
            int hostId = GetHostId();
            await Clients.OthersInGroup(hostId.ToString()).ItemDeleted(id);
            await db.DeleteItem(id, hostId);
        }

        public async Task CartItemMoved(int id, int newIndex)
        {
            int hostId = GetHostId();
            await Clients.OthersInGroup(hostId.ToString()).ItemMoved(id, newIndex);
        }

        public async Task<HubResponse> LeaveGroup()
        {
            int hostId = GetHostId();
            if (hostId >= 0)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, hostId.ToString());
                await Clients.OthersInGroup(hostId.ToString()).GetMessage($"{GetUserEmail(Context)} has left the group.");
                return new HubResponse() { SuccessMessage = "You have left the group." };
            }
            else
            {
                return new HubResponse() { ErrorMessage = "You are not part of any shopping cart." };
            }
        }

        private static int GetUserId(HubCallerContext context)
        {
            return int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }

        private static string GetUserEmail(HubCallerContext context)
        {
            return context.User.FindFirst(ClaimTypes.Email).Value;
        }
    }
}

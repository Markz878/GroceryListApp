using GroceryListHelper.Server.Data;
using GroceryListHelper.Server.Models;
using GroceryListHelper.Shared;
using GroceryListHelper.Shared.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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
        private readonly GroceryStoreDbContext db;
        private readonly CartHubService cartHubService;

        public CartHub(GroceryStoreDbContext db, CartHubService cartHubService)
        {
            this.db = db;
            this.cartHubService = cartHubService;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (exception == null)
            {
                int hostId = GetHostId();
                cartHubService.GroupAllowedEmails.Remove(hostId);
            }
            return base.OnDisconnectedAsync(exception);
        }

        public async Task<HubResponse> CreateGroup(List<string> allowedUsers)
        {
            if (allowedUsers.Count == 0)
            {
                return new HubResponse() { IsSuccess = false, Message = "There are no allowed users for your cart" };
            }
            await Groups.AddToGroupAsync(Context.ConnectionId, GetUserId(Context).ToString());
            string otherUsers = string.Join(", ", allowedUsers);
            allowedUsers.Add(GetUserEmail(Context));
            cartHubService.GroupAllowedEmails[GetUserId(Context)] = allowedUsers;
            return new HubResponse() { IsSuccess = true, Message = $"You are sharing your cart to {otherUsers}." };
        }

        public async Task<HubResponse> JoinGroup(string hostEmail)
        {
            int hostId = GetIdFromEmail(hostEmail);
            if (hostId < 0)
            {
                return new HubResponse() { IsSuccess = false, Message = "No user with that email." };
            }
            if (cartHubService.GroupAllowedEmails.TryGetValue(hostId, out List<string> emails))
            {
                if (emails.Contains(GetUserEmail(Context)))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, hostId.ToString());
                    await Clients.Caller.GetCart((await db.CartProducts.Where(x => x.UserId.Equals(hostId)).ToListAsync()).ConvertAll(x => x as CartProductCollectable));
                }
                else
                {
                    return new HubResponse() { IsSuccess = false, Message = "You are not allowed to join this cart." };
                }
                await Clients.OthersInGroup(hostId.ToString()).GetMessage($"{GetUserEmail(Context)} has joined {hostEmail}'s cart.");
                return new HubResponse() { IsSuccess = true, Message = $"You have joined {hostEmail}'s cart" };
            }
            else
            {
                return new HubResponse() { IsSuccess = false, Message = "There is no cart hosted by that user." };
            }
        }

        public async Task<bool> CartItemAdded(CartProductCollectable product)
        {
            int hostId = GetHostId();
            CartProductDbModel cartDbProduct = new CartProductDbModel() { Amount = product.Amount, Name = product.Name, UnitPrice = product.UnitPrice, UserId = hostId };
            db.CartProducts.Add(cartDbProduct);
            await db.SaveChangesAsync();
            product.Id = cartDbProduct.Id;
            await Clients.OthersInGroup(hostId.ToString()).ItemAdded(product);
            return true;
        }

        public async Task<bool> CartItemModified(CartProductCollectable product)
        {
            int hostId = GetHostId();
            CartProductDbModel cartDbProduct = db.CartProducts.First(x => x.Id.Equals(product.Id));
            cartDbProduct.Amount = product.Amount;
            cartDbProduct.Name = product.Name;
            cartDbProduct.UnitPrice = product.UnitPrice;
            await db.SaveChangesAsync();
            await Clients.OthersInGroup(hostId.ToString()).ItemModified(product);
            return true;
        }

        public async Task<bool> CartItemCollected(int id)
        {
            int hostId = GetHostId();
            db.CartProducts.First(x => x.Id.Equals(id)).IsCollected ^= true;
            await Clients.OthersInGroup(hostId.ToString()).ItemCollected(id);
            await db.SaveChangesAsync();
            return true;
        }

        private int GetHostId()
        {
            string userEmail = GetUserEmail(Context);
            int hostId = cartHubService.GroupAllowedEmails.FirstOrDefault(x => x.Value.Contains(userEmail)).Key;
            return hostId;
        }

        public async Task<bool> CartItemDeleted(int id)
        {
            int hostId = GetHostId();
            db.CartProducts.Remove(db.CartProducts.First(x => x.Id.Equals(id) && x.UserId.Equals(hostId)));
            await Clients.OthersInGroup(hostId.ToString()).ItemDeleted(id);
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<HubResponse> LeaveGroup()
        {
            int hostId = GetHostId();
            if (hostId >= 0)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, hostId.ToString());
                await Clients.OthersInGroup(hostId.ToString()).GetMessage($"{Context.User.FindFirst(ClaimTypes.Email).Value} has left the group.");
                return new HubResponse() { IsSuccess = true, Message = "You have left the group." };
            }
            else
            {
                return new HubResponse() { IsSuccess = false, Message = "You are not part of any shopping cart." };
            }
        }

        private int GetIdFromEmail(string email) => db.Users.FirstOrDefault(x => x.Email.Equals(email))?.Id ?? -1;
        private static int GetUserId(HubCallerContext context) => int.Parse(context.User.FindFirst("Id").Value);
        private static string GetUserEmail(HubCallerContext context) => context.User.FindFirst(ClaimTypes.Email).Value;
    }
}

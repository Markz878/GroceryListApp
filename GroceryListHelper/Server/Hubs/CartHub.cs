using GroceryListHelper.Server.Data;
using GroceryListHelper.Server.Models;
using GroceryListHelper.Shared;
using GroceryListHelper.Shared.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GroceryListHelper.Server.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CartHub : Hub<ICartHubClient>, ICartHub
    {
        private readonly GroceryStoreDbContext db;

        public CartHub(GroceryStoreDbContext db)
        {
            this.db = db;
        }

        public async Task<string> CreateGroup(string[] allowedUsers)
        {
            if (allowedUsers.Length == 0)
            {
                throw new ArgumentException("There are no allowed users for your cart");
            }
            await Groups.AddToGroupAsync(Context.ConnectionId, GetUserId(Context));
            Context.Items[GetUserId(Context)] = allowedUsers;
            if (allowedUsers.Length > 1)
            {
                return $"You are sharing your cart to {allowedUsers.Length} users.";
            }
            else
            {
                return $"You are sharing your cart to {allowedUsers[0]}";
            }
        }

        public async Task<string> JoinGroup(string hostEmail)
        {
            int hostId = GetUserId(hostEmail);
            if (hostId<0)
            {
                throw new ArgumentException("No user with that email.");
            }
            if (Context.Items.TryGetValue(hostId, out object emails))
            {
                if ((emails as string[]).Contains(GetUserEmail(Context)))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, hostId.ToString());
                    await Clients.Caller.GetCart((await db.CartProducts.Where(x => x.UserId.Equals(hostId)).ToListAsync()).ConvertAll(x => x as CartProductCollectable));
                }
                else
                {
                    throw new UnauthorizedAccessException("You are not allowed to join this cart.");
                }
                await Clients.OthersInGroup(hostId.ToString()).GetMessage($"{GetUserEmail(Context)} has joined {hostEmail}'s cart.");
                return $"You have joined {hostEmail}'s cart";
            }
            else
            {
                throw new ArgumentException("There is no cart hosted from that user.");
            }

        }

        public async Task<bool> CartItemAdded(string hostEmail, CartProductCollectable product)
        {
            int hostId = GetUserId(hostEmail);
            CartProductDbModel cartDbProduct = new CartProductDbModel() { Amount = product.Amount, Name = product.Name, UnitPrice = product.UnitPrice, UserId = hostId };
            db.CartProducts.Add(cartDbProduct);
            await db.SaveChangesAsync();
            product.Id = cartDbProduct.Id;
            await Clients.OthersInGroup(GetUserEmail(Context)).ItemAdded(product);
            return true;
        }

        public async Task<bool> CartItemModified(string hostEmail, CartProductCollectable product)
        {
            int hostId = GetUserId(hostEmail);
            CartProductDbModel cartDbProduct = db.CartProducts.First(x => x.Id.Equals(product.Id));
            cartDbProduct.Amount = product.Amount;
            cartDbProduct.Name = product.Name;
            cartDbProduct.UnitPrice = product.UnitPrice;
            await db.SaveChangesAsync();
            await Clients.OthersInGroup(GetUserEmail(Context)).ItemModified(product);
            return true;

        }

        public async Task<bool> CartItemCollected(int id)
        {
            db.CartProducts.First(x => x.Id.Equals(id)).IsCollected ^= true;
            await Clients.OthersInGroup(GetUserEmail(Context)).ItemCollected(id);
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CartItemDeleted(int id)
        {
            db.CartProducts.Remove(db.CartProducts.First(x => x.Id.Equals(id)));
            await Clients.OthersInGroup(GetUserEmail(Context)).ItemDeleted(id);
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<string> LeaveGroup(string hostEmail)
        {
            int hostId = GetUserId(hostEmail);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, hostId.ToString());
            await Clients.OthersInGroup(hostId.ToString()).GetMessage($"{Context.User.FindFirst(ClaimTypes.Email).Value} has left the group {hostEmail}.");
            return $"You have left the group {hostEmail}.";
        }

        private int GetUserId(string email) => db.Users.FirstOrDefault(x => x.Email.Equals(email))?.Id ?? -1;
        private static string GetUserId(HubCallerContext context) => context.User.FindFirst("Id").Value;
        private static string GetUserEmail(HubCallerContext context) => context.User.FindFirst(ClaimTypes.Email).Value;
    }
}

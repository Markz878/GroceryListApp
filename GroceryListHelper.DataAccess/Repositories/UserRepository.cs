using GroceryListHelper.DataAccess.HelperMethods;
using GroceryListHelper.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GroceryListHelper.DataAccess.Repositories
{
    public class UserRepository
    {
        private readonly GroceryStoreDbContext db;
        public UserRepository(GroceryStoreDbContext db)
        {
            this.db = db;
        }

        public async Task<string> RemoveRefreshToken(int id)
        {
            try
            {
                UserDbModel user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    return "User not found.";
                }
                else
                {
                    user.RefreshToken = string.Empty;
                    await db.SaveChangesAsync();
                    return null;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<UserDbModel> GetUserFromEmail(string email)
        {
            UserDbModel userDbModel = await db.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            return userDbModel;
        }

        public async Task<int> GetIdFromEmail(string email)
        {
            UserDbModel userDbModel = await db.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            if (userDbModel == null)
            {
                return -1;
            }
            return userDbModel.Id;
        }

        public async Task<string> AddUser(string email, string password)
        {
            try
            {
                byte[] salt = PasswordHelper.GenerateSalt();
                UserDbModel user = new() { Email = email, Salt = salt, PasswordHash = PasswordHelper.HashPassword(password, salt) };
                db.Users.Add(user);
                await db.SaveChangesAsync();
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> ChangePassword(string email, string currentPassword, string newPassword)
        {
            UserDbModel user = await db.Users.Where(x => x.Email == email).FirstOrDefaultAsync();
            if (user == null)
            {
                return "User not found";
            }
            else
            {
                if (user.PasswordHash.Equals(PasswordHelper.HashPassword(currentPassword, user.Salt)))
                {
                    user.Salt = PasswordHelper.GenerateSalt();
                    user.PasswordHash = PasswordHelper.HashPassword(newPassword, user.Salt);
                    await db.SaveChangesAsync();
                    return null;
                }
                return "Invalid password";
            }
        }

        public async Task<string> DeleteUser(string email, string password)
        {
            UserDbModel user = await db.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            if (user == null)
            {
                return "User not found";
            }
            else
            {
                if (user.PasswordHash.Equals(PasswordHelper.HashPassword(password, user.Salt)))
                {
                    db.Users.Remove(user);
                    await db.SaveChangesAsync();
                    return null;
                }
            }
            return "Invalid username or password";
        }

        public async Task<string> RenewRefreshToken(string email, string refreshToken)
        {
            try
            {
                UserDbModel user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    return "User not found.";
                }
                else
                {
                    user.RefreshToken = refreshToken;
                    await db.SaveChangesAsync();
                    return null;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}

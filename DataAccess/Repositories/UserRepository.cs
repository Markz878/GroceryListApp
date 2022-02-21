using GroceryListHelper.DataAccess.HelperMethods;
using GroceryListHelper.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GroceryListHelper.DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private readonly GroceryStoreDbContext db;
    private readonly ILogger<UserRepository> logger;

    public UserRepository(GroceryStoreDbContext db, ILogger<UserRepository> logger)
    {
        this.db = db;
        this.logger = logger;
    }

    public async Task<string> RemoveRefreshToken(string id)
    {
        try
        {
            UserDbModel user = await GetUserFromId(id);
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

    public async Task<UserDbModel> GetUserFromId(string id)
    {
        UserDbModel userDbModel = await db.Users.FirstOrDefaultAsync(u => u.Id.Equals(id));
        return userDbModel;
    }

    public async Task<UserDbModel> GetUserFromEmail(string email)
    {
        UserDbModel userDbModel = await db.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
        return userDbModel;
    }

    public async Task<UserDbModel> AddUser(string email, string password)
    {
        try
        {
            byte[] salt = PasswordHelper.GenerateSalt();
            UserDbModel user = new() { Email = email, Salt = salt, PasswordHash = PasswordHelper.HashPassword(password, salt) };
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return user;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating user.");
            return null;
        }
    }

    public async Task<string> ChangeEmail(string id, string newEmail, string password)
    {
        UserDbModel user = await GetUserFromId(id);
        if (user == null)
        {
            return "User not found";
        }
        else
        {
            if (user.PasswordHash.Equals(PasswordHelper.HashPassword(password, user.Salt)))
            {
                user.Email = newEmail;
                await db.SaveChangesAsync();
                return null;
            }
            return "Invalid password";
        }
    }

    public async Task<string> ChangePassword(string id, string currentPassword, string newPassword)
    {
        UserDbModel user = await GetUserFromId(id);
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
            return "Invalid username or password";
        }
    }

    public async Task<string> DeleteUser(string id, string password)
    {
        UserDbModel user = await GetUserFromId(id);
        if (user == null)
        {
            return "Could not delete user";
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

    public async Task<string> UpdateRefreshToken(string id, string refreshToken)
    {
        try
        {
            UserDbModel user = await GetUserFromId(id);
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
            logger.LogError(ex, "Error renewing refresh token.");
            return ex.Message;
        }
    }

    public async Task<List<string>> GetCartHostAllowedEmails(string hostId)
    {
        var result = await db.UserCartGroups.Where(x => x.HostId == hostId).Select(x=>x.JoinerEmail).ToListAsync();
        return result;
    }

    public async Task CreateGroupAllowedEmails(string hostId, List<string> allowedUserEmails)
    {
        foreach (var userEmail in allowedUserEmails)
        {
            db.UserCartGroups.Add(new UserCartGroupDbModel()
            {
                HostId = hostId,
                JoinerEmail = userEmail,
            });
        }
        await db.SaveChangesAsync();
    }

    public async Task<string> GetUsersCartHostId(string email)
    {
        UserCartGroupDbModel host = await db.UserCartGroups.FirstOrDefaultAsync(x=>x.JoinerEmail == email);
        return host?.HostId;
    }

    public async Task RemoveCartGroup(string hostId)
    {
        db.UserCartGroups.RemoveRange(db.UserCartGroups.Where(x => x.HostId == hostId));
        await db.SaveChangesAsync();
    }
}

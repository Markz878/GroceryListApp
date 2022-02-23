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

    public async Task<string> GetHostIdFromHostEmail(string hostEmail)
    {
        string hostId = await db.UserCartGroups.AsNoTracking().Where(u => u.HostEmail.Equals(hostEmail)).Select(x => x.HostId).FirstOrDefaultAsync();
        return hostId;
    }

    public async Task<List<string>> GetCartHostAllowedEmails(string hostId)
    {
        List<string> result = await db.UserCartGroups.AsNoTracking().Where(x => x.HostId == hostId).Select(x => x.JoinerEmail).ToListAsync();
        return result;
    }

    public async Task CreateGroupAllowedEmails(string hostId, string hostEmail, List<string> allowedUserEmails)
    {
        foreach (string userEmail in allowedUserEmails)
        {
            db.UserCartGroups.Add(new UserCartGroupDbModel()
            {
                HostId = hostId,
                HostEmail = hostEmail,
                JoinerEmail = userEmail,
            });
        }
        await db.SaveChangesAsync();
    }

    public async Task<string> GetUsersCartHostId(string email)
    {
        string hostId = await db.UserCartGroups.AsNoTracking().Where(x => x.JoinerEmail == email).Select(x => x.HostId).FirstOrDefaultAsync();
        return hostId;
    }

    public async Task RemoveCartGroup(string hostId)
    {
        db.UserCartGroups.RemoveRange(db.UserCartGroups.Where(x => x.HostId == hostId));
        await db.SaveChangesAsync();
    }
}

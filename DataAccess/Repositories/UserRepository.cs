using GroceryListHelper.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GroceryListHelper.DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private readonly GroceryStoreDbContext db;

    public UserRepository(GroceryStoreDbContext db)
    {
        this.db = db;
    }

    public async Task<Guid> GetHostIdFromHostEmail(string hostEmail)
    {
        Guid hostId = await db.UserCartGroups.AsNoTracking().Where(u => u.HostEmail.Equals(hostEmail)).Select(x => x.HostId).FirstOrDefaultAsync();
        return hostId;
    }

    public async Task<List<string>> GetCartHostAllowedEmails(Guid hostId)
    {
        List<string> result = await db.UserCartGroups.AsNoTracking().Where(x => x.HostId == hostId).Select(x => x.JoinerEmail).ToListAsync();
        return result;
    }

    public async Task CreateGroupAllowedEmails(Guid hostId, string hostEmail, List<string> allowedUserEmails)
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

    public async Task<Guid> GetUsersCartHostId(string email)
    {
        Guid hostId = await db.UserCartGroups.AsNoTracking().Where(x => x.JoinerEmail == email).Select(x => x.HostId).FirstOrDefaultAsync();
        return hostId;
    }

    public async Task RemoveCartGroup(Guid hostId)
    {
        List<UserCartGroupDbModel> groups = await db.UserCartGroups.Where(x => x.HostId == hostId).ToListAsync();
        db.UserCartGroups.RemoveRange(groups);
        await db.SaveChangesAsync();
    }
}

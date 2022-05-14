namespace GroceryListHelper.DataAccess.Repositories;

public interface IUserRepository
{
    Task<Guid> GetHostIdFromHostEmail(string email);
    Task<List<string>> GetCartHostAllowedEmails(Guid hostId);
    Task CreateGroupAllowedEmails(Guid hostId, string hostEmail, List<string> allowedUserIds);
    Task<Guid> GetUsersCartHostId(string userEmail);
    Task RemoveCartGroup(Guid hostId);
}

namespace GroceryListHelper.DataAccess.Repositories;

public interface IUserRepository
{
    Task<string> GetHostIdFromHostEmail(string email);
    Task<List<string>> GetCartHostAllowedEmails(string hostId);
    Task CreateGroupAllowedEmails(string hostId, string hostEmail, List<string> allowedUserIds);
    Task<string> GetUsersCartHostId(string userEmail);
    Task RemoveCartGroup(string hostId);
}

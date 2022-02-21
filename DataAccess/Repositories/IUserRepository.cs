using GroceryListHelper.DataAccess.Models;

namespace GroceryListHelper.DataAccess.Repositories;

public interface IUserRepository
{
    Task<UserDbModel> AddUser(string email, string password);
    Task<string> ChangeEmail(string id, string newEmail, string password);
    Task<string> ChangePassword(string id, string currentPassword, string newPassword);
    Task<string> DeleteUser(string id, string password);
    Task<UserDbModel> GetUserFromEmail(string email);
    Task<UserDbModel> GetUserFromId(string id);
    Task<string> RemoveRefreshToken(string id);
    Task<string> UpdateRefreshToken(string id, string refreshToken);
    Task<List<string>> GetCartHostAllowedEmails(string hostId);
    Task CreateGroupAllowedEmails(string hostId, List<string> allowedUserIds);
    Task<string> GetUsersCartHostId(string userEmail);
    Task RemoveCartGroup(string hostId);
}

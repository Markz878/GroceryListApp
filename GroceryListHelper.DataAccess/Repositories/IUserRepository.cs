using GroceryListHelper.DataAccess.Models;
using System.Threading.Tasks;

namespace GroceryListHelper.DataAccess.Repositories
{
    public interface IUserRepository
    {
        Task<UserDbModel> AddUser(string email, string password);
        Task<string> ChangeEmail(int id, string newEmail, string password);
        Task<string> ChangePassword(int id, string currentPassword, string newPassword);
        Task<string> DeleteUser(int id, string password);
        Task<UserDbModel> GetUserFromEmail(string email);
        Task<UserDbModel> GetUserFromId(int id);
        Task<string> RemoveRefreshToken(int id);
        Task<string> UpdateRefreshToken(int id, string refreshToken);
    }
}
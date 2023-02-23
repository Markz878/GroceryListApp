namespace GroceryListHelper.DataAccess.Repositories;

public interface IUserRepository
{
    Task<bool> AddUser(string email, Guid id, string? name);
    Task<bool> CheckIfUserExists(string email);
}
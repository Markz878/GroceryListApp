using Azure;
using Azure.Data.Tables;
using GroceryListHelper.Core.RepositoryContracts;
using GroceryListHelper.DataAccess.Models;

namespace GroceryListHelper.DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private readonly TableClient db;

    public UserRepository(TableServiceClient db)
    {
        this.db = db.GetTableClient(UserDbModel.GetTableName());
    }

    public async Task<bool> AddUser(string email, Guid id, string? name)
    {
        try
        {
            await db.AddEntityAsync(new UserDbModel() { Email = email, Id = id, Name = name });
            return true;
        }
        catch (RequestFailedException ex) when (ex.Status is 409)
        {
            return false;
        }
    }

    public async Task<bool> CheckIfUserExists(string email)
    {
        NullableResponse<UserDbModel> x = await db.GetEntityIfExistsAsync<UserDbModel>(email, email, Array.Empty<string>());
        return x.HasValue;
    }
}

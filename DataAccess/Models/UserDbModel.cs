using GroceryListHelper.Shared.Models.Authentication;

namespace GroceryListHelper.DataAccess.Models;

public class UserDbModel : UserModel
{
    public string PasswordHash { get; set; }
    public byte[] Salt { get; set; }
    public string RefreshToken { get; set; }
}

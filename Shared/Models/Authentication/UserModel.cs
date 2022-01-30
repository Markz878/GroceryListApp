namespace GroceryListHelper.Shared.Models.Authentication;

public class UserModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Email { get; set; }
}

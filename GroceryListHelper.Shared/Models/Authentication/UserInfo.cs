namespace GroceryListHelper.Shared.Models.Authentication;

public sealed class UserInfo
{
    public bool IsAuthenticated { get; set; }
    public IEnumerable<ClaimValue> Claims { get; set; } = [];
}

namespace GroceryListHelper.Shared.Models.Authentication;

public class UserInfo
{
    public static readonly UserInfo Anonymous = new();
    public bool IsAuthenticated { get; set; }
    public IEnumerable<ClaimValue>? Claims { get; set; }
}

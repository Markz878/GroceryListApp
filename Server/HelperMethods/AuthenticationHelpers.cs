namespace GroceryListHelper.Server.HelperMethods;

public static class AuthenticationHelpers
{
    public static Guid? GetUserId(this ClaimsPrincipal user)
    {
        string? stringId = user.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");
        return stringId == null ? null : Guid.Parse(stringId.Trim('"'));
    }
}

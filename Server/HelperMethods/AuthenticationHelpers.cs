namespace GroceryListHelper.Server.HelperMethods;

public static class AuthenticationHelpers
{
    public static string GetUserEmail(this ClaimsPrincipal user)
    {
        string? email = user.GetDisplayName();
        ArgumentNullException.ThrowIfNull(email);
        return email;
    }

    public static Guid? GetUserId(this ClaimsPrincipal user)
    {
        string? stringId = user.GetObjectId();
        return stringId == null ? null : Guid.Parse(stringId.Trim('"'));
    }

    public static string? GetUserName(this ClaimsPrincipal user)
    {
        string? name = user.FindFirstValue("name");
        return name;
    }
}

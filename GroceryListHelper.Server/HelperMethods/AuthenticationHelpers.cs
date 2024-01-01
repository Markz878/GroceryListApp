using GroceryListHelper.Server.Models.Authentication;

namespace GroceryListHelper.Server.HelperMethods;

public static class AuthenticationHelpers
{
    public static Guid GetUserId(this ClaimsPrincipal? user)
    {
        string? userId = user?.Claims.FirstOrDefault(x => x.Type == AuthenticationConstants.IdClaimName)?.Value;
        return string.IsNullOrEmpty(userId) ? Guid.Empty : Guid.Parse(userId);
    }

    public static string? GetUserEmail(this ClaimsPrincipal user)
    {
        string? email = user.Claims.FirstOrDefault(x => x.Type == AuthenticationConstants.EmailClaimName)?.Value;
        return email;
    }

    public static string? GetUserName(this ClaimsPrincipal user)
    {
        string? name = user.Claims.FirstOrDefault(x => x.Type == AuthenticationConstants.NameClaimName)?.Value;
        return name;
    }

    public static UserInfo GetUserInfo(this ClaimsPrincipal user)
    {
        return new UserInfo()
        {
            IsAuthenticated = user.Identity?.IsAuthenticated == true,
            Claims = user.Claims.Select(x => new ClaimValue(x.Type, x.Value)).ToList(),
        };
    }
}

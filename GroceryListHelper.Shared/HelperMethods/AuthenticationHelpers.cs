using GroceryListHelper.Shared.Models.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace GroceryListHelper.Shared.HelperMethods;

public static class AuthenticationHelpers
{
    public static Guid GetUserId(this ClaimsPrincipal? user)
    {
        string? userId = user?.Claims.FirstOrDefault(x => x.Type == AuthenticationConstants.IdClaimName)?.Value;
        return string.IsNullOrEmpty(userId) ? Guid.Empty : Guid.Parse(userId);
    }

    public static Guid? GetUserId(this UserInfo userInfo)
    {
        string? userId = userInfo.Claims.FirstOrDefault(x => x.Type == AuthenticationConstants.IdClaimName)?.Value;
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

    public static async Task<UserInfo> GetUserInfo(this Task<AuthenticationState> authenticationStateTask)
    {
        AuthenticationState authenticationState = await authenticationStateTask;
        return new UserInfo()
        {
            IsAuthenticated = authenticationState.User.Identity?.IsAuthenticated == true,
            Claims = authenticationState.User.Claims.Select(x => new ClaimValue(x.Type, x.Value)).ToList(),
        };
    }

    public static string? GetUserEmail(this UserInfo user)
    {
        string? email = user.Claims.FirstOrDefault(x => x.Type == AuthenticationConstants.EmailClaimName)?.Value;
        return email;
    }

    public static string? GetUserName(this UserInfo user)
    {
        string? name = user.Claims.FirstOrDefault(x => x.Type == AuthenticationConstants.NameClaimName)?.Value;
        return name;
    }
}

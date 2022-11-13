using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GroceryListHelper.Server.Endpoints;

public static class AccountEndpointsMapper
{
    public static void AddAccountEndpoints(this RouteGroupBuilder builder)
    {
        RouteGroupBuilder accountGroup = builder.MapGroup("account").WithTags("Account");
        accountGroup.MapGet("login", Login);
        accountGroup.MapPost("logout", Logout).RequireAuthorization();
        accountGroup.MapGet("user", GetUserInfo);
    }

    internal static ChallengeHttpResult Login(string? returnUrl)
    {
        return TypedResults.Challenge(new AuthenticationProperties
        {
            RedirectUri = !string.IsNullOrEmpty(returnUrl) ? returnUrl : "/"
        });
    }

    internal static SignOutHttpResult Logout()
    {
        return TypedResults.SignOut(new AuthenticationProperties { RedirectUri = "/" }, new List<string>()
            {
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme
            });
    }

    private static readonly string[] returnClaimTypes = new[] { "name", "preferred_username", "http://schemas.microsoft.com/identity/claims/objectidentifier" };
    internal static Ok<UserInfo> GetUserInfo(ClaimsPrincipal claimsPrincipal)
    {
        if (claimsPrincipal.Identity?.IsAuthenticated == false)
        {
            return TypedResults.Ok(UserInfo.Anonymous);
        }
        UserInfo userInfo = new()
        {
            IsAuthenticated = true
        };
        if (claimsPrincipal.Claims.Any())
        {
            List<ClaimValue> userInfoClaims = new();
            foreach (Claim claim in claimsPrincipal.FindAll(x => returnClaimTypes.Contains(x.Type)))
            {
                userInfoClaims.Add(new ClaimValue(claim.Type, claim.Value));
            }
            userInfo.Claims = userInfoClaims;
        }
        return TypedResults.Ok(userInfo);
    }
}

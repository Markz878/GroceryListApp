using GroceryListHelper.Server.HelperMethods;
using GroceryListHelper.Server.Models.Authentication;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace GroceryListHelper.Server.Endpoints;

public static class AccountEndpointsMapper
{
    public static RouteGroupBuilder AddAccountEndpoints(this RouteGroupBuilder builder)
    {
        RouteGroupBuilder accountGroup = builder.MapGroup("account").WithTags("Account");
        accountGroup.MapGet("user", GetUserInfo).AllowAnonymous();
        accountGroup.MapGet("token", GetToken);
        accountGroup.MapGet("login", Login).AllowAnonymous();
        accountGroup.MapPost("logout", Logout);
        accountGroup.MapGet("signout", PostLogoutRedirect).AllowAnonymous().ExcludeFromDescription();
        return builder;
    }

    private static readonly CookieOptions csrfTokenOptions = new() { HttpOnly = false, Secure = true };
    public static NoContent GetToken(IAntiforgery antiforgery, HttpContext context)
    {
        AntiforgeryTokenSet tokens = antiforgery.GetAndStoreTokens(context);
        if (!string.IsNullOrWhiteSpace(tokens.RequestToken))
        {
            context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken, csrfTokenOptions);
        }
        return TypedResults.NoContent();
    }

    public static Ok<UserInfo> GetUserInfo(ClaimsPrincipal user)
    {
        return TypedResults.Ok(user.GetUserInfo());
    }

    public static ChallengeHttpResult Login(string? returnUrl)
    {
        return TypedResults.Challenge(new AuthenticationProperties
        {
            RedirectUri = !string.IsNullOrEmpty(returnUrl) ? returnUrl : "/"
        });
    }

    private static readonly IList<string> _authSchemes =
    [
        CookieAuthenticationDefaults.AuthenticationScheme,
        OpenIdConnectDefaults.AuthenticationScheme
    ];
    public static SignOutHttpResult Logout()
    {
        return TypedResults.SignOut(authenticationSchemes: _authSchemes);
    }

    public static RedirectHttpResult PostLogoutRedirect()
    {
        return TypedResults.Redirect("/");
    }
}

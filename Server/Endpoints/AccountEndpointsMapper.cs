using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;

namespace GroceryListHelper.Server.Endpoints;

public static class AccountEndpointsMapper
{
    public static void AddAccountEndpoints(this RouteGroupBuilder builder)
    {
        RouteGroupBuilder accountGroup = builder.MapGroup("account");

        accountGroup.MapGet("login", (string? returnUrl) =>
            {
                var challengeResult = Results.Challenge(new AuthenticationProperties
                {
                    RedirectUri = !string.IsNullOrEmpty(returnUrl) ? returnUrl : "/"
                });
                return challengeResult;
            });

        accountGroup.MapPost("logout", () =>
            {
                return Results.SignOut(
                    new AuthenticationProperties { RedirectUri = "/" }, new List<string>()
                    {
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        OpenIdConnectDefaults.AuthenticationScheme
                    });
            })
            .RequireAuthorization().AddEndpointFilter<AntiForgeryTokenFilter>();
    }
}

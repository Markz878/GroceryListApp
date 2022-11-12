using System.Security.Claims;

namespace GroceryListHelper.Server.Endpoints;

public static class UserEndpointsMapper
{
    private static readonly string[] returnClaimTypes = new[] { "name", "preferred_username", "http://schemas.microsoft.com/identity/claims/objectidentifier" };
    public static void AddUserEndpoints(this RouteGroupBuilder builder)
    {
        builder.MapGet("user", (ClaimsPrincipal claimsPrincipal) =>
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
        });
    }
}
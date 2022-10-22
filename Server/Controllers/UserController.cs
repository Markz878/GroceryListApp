using GroceryListHelper.Shared.Models.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace GroceryListHelper.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public IActionResult GetCurrentUser()
    {
        return Ok(User.Identity.IsAuthenticated ? CreateUserInfo(User) : UserInfo.Anonymous);
    }

    private static readonly string[] returnClaimTypes = new[] { "name", "preferred_username", "http://schemas.microsoft.com/identity/claims/objectidentifier" };
    private static UserInfo CreateUserInfo(ClaimsPrincipal claimsPrincipal)
    {
        if (!claimsPrincipal.Identity.IsAuthenticated)
        {
            return UserInfo.Anonymous;
        }

        UserInfo userInfo = new()
        {
            IsAuthenticated = true
        };

        if (claimsPrincipal.Identity is ClaimsIdentity claimsIdentity)
        {
            userInfo.NameClaimType = claimsIdentity.NameClaimType;
            userInfo.RoleClaimType = claimsIdentity.RoleClaimType;
        }
        else
        {
            userInfo.NameClaimType = "name";
            userInfo.RoleClaimType = "role";
        }

        if (claimsPrincipal.Claims.Any())
        {
            List<ClaimValue> userInfoClaims = new();
            IEnumerable<Claim> returnClaims = claimsPrincipal.FindAll(x => returnClaimTypes.Contains(x.Type));
            foreach (Claim claim in returnClaims)
            {
                userInfoClaims.Add(new ClaimValue(claim.Type, claim.Value));
            }
            userInfo.Claims = userInfoClaims;
        }

        return userInfo;
    }
}

using GroceryListHelper.Shared.Models.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            List<ClaimValue> claims = new();
            IEnumerable<Claim> nameClaims = claimsPrincipal.FindAll(userInfo.NameClaimType);
            foreach (Claim claim in nameClaims)
            {
                claims.Add(new ClaimValue(userInfo.NameClaimType, claim.Value));
            }

            // Uncomment this code if you want to send additional claims to the client.
            foreach (var claim in claimsPrincipal.Claims.Except(nameClaims))
            {
                claims.Add(new ClaimValue(claim.Type, claim.Value));
            }

            userInfo.Claims = claims;
        }

        return userInfo;
    }
}

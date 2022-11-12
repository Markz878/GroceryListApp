//namespace GroceryListHelper.Server.Controllers;

//[Route("api/[controller]")]
//[ApiController]
//public class UserController : ControllerBase
//{
//    [HttpGet]
//    public IActionResult GetCurrentUser()
//    {
//        return Ok(CreateUserInfo(User));
//    }

//    private static readonly string[] returnClaimTypes = new[] { "name", "preferred_username", "http://schemas.microsoft.com/identity/claims/objectidentifier" };
//    private static UserInfo CreateUserInfo(ClaimsPrincipal claimsPrincipal)
//    {
//        if (claimsPrincipal.Identity?.IsAuthenticated == false)
//        {
//            return UserInfo.Anonymous;
//        }
//        UserInfo userInfo = new()
//        {
//            IsAuthenticated = true
//        };
//        if (claimsPrincipal.Claims.Any())
//        {
//            List<ClaimValue> userInfoClaims = new();
//            foreach (Claim claim in claimsPrincipal.FindAll(x => returnClaimTypes.Contains(x.Type)))
//            {
//                userInfoClaims.Add(new ClaimValue(claim.Type, claim.Value));
//            }
//            userInfo.Claims = userInfoClaims;
//        }
//        return userInfo;
//    }
//}

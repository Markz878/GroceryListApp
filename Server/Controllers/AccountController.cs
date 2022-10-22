using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;

namespace GroceryListHelper.Server.Controllers;

[Route("api/[controller]/[action]")]
public class AccountController : ControllerBase
{
    [HttpGet]
    public ActionResult Login(string returnUrl)
    {
        ChallengeResult challengeResult = Challenge(new AuthenticationProperties
        {
            RedirectUri = !string.IsNullOrEmpty(returnUrl) ? returnUrl : "/"
        });
        return challengeResult;
    }

    [Authorize]
    [HttpPost]
    public IActionResult Logout()
    {
        return SignOut(
            new AuthenticationProperties { RedirectUri = "/" },
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIdConnectDefaults.AuthenticationScheme);
    }
}

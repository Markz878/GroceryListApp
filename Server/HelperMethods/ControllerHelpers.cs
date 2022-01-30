using System.Security.Claims;

namespace GroceryListHelper.Server.HelperMethods;

public static class ControllerHelpers
{
    public static string GetUserId(this ClaimsPrincipal user)
    {
        return user.FindFirstValue("id");
    }
}

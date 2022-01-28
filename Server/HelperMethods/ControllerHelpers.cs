using System;
using System.Security.Claims;

namespace GroceryListHelper.Server.HelperMethods;

public static class ControllerHelpers
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        string idClaim = user.FindFirstValue("id");
        if (string.IsNullOrEmpty(idClaim))
        {
            return -1;
        }
        return int.Parse(idClaim);
    }
}

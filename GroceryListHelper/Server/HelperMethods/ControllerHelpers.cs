using System.Security.Claims;

namespace GroceryListHelper.Server.HelperMethods
{
    public static class ControllerHelpers
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            return int.Parse(user.FindFirst("Id").Value);
        }

        public static string GetUserEmail(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.Email);
        }
    }
}

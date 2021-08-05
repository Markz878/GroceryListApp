using System.Security.Claims;

namespace GroceryListHelper.Server.HelperMethods
{
    public static class ControllerHelpers
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier).Value);
        }
    }
}

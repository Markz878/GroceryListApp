using System.Security.Claims;

namespace GroceryListHelper.Server.HelperMethods;

public static class ControllerHelpers
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        string textId = user.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");
        Guid guidId = Guid.Parse(textId.Trim('"'));
        return guidId;
    }
}

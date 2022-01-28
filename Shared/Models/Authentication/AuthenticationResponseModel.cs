using GroceryListHelper.Shared.Models.BaseModels;

namespace GroceryListHelper.Shared.Models.Authentication;

public class AuthenticationResponseModel : BaseResponse
{
    public string AccessToken { get; set; }
}

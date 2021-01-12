namespace GroceryListHelper.Shared
{
    public class LoginResponseModel : BaseAPIResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}

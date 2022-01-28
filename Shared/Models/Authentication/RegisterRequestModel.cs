namespace GroceryListHelper.Shared.Models.Authentication;

public class RegisterRequestModel : UserCredentialsModel
{
    public string ConfirmPassword { get; set; }
}

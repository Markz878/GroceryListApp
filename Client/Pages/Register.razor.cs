using GroceryListHelper.Client.Services;
using GroceryListHelper.Shared.Models.Authentication;
using Microsoft.AspNetCore.Components;


namespace GroceryListHelper.Client.Pages;

public partial class Register
{
    [Inject] public AuthenticationService AuthenticationService { get; set; }
    [Inject] public NavigationManager Navigation { get; set; }

    private readonly RegisterRequestModel registerRequest = new();
    public string Message { get; set; } = string.Empty;
    private bool isBusy;

    private async Task RegisterMethod()
    {
        try
        {
            isBusy = true;
            string error = await AuthenticationService.Register(registerRequest);
            if (!string.IsNullOrEmpty(error))
            {
                Message = error;
            }
            else
            {
                Navigation.NavigateTo("/", true);
            }
        }
        catch (Exception ex)
        {
            Message = ex.Message;
        }
        finally
        {
            isBusy = false;
        }
    }
}

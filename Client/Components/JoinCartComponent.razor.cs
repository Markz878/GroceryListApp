using GroceryListHelper.Client.HelperMethods;
using GroceryListHelper.Client.ViewModels;
using GroceryListHelper.Shared.Interfaces;
using GroceryListHelper.Shared.Models.BaseModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

namespace GroceryListHelper.Client.Components;

public class JoinCartComponentBase : BasePage<IndexViewModel>
{
    [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }
    [Inject] public ModalViewModel ModalViewModel { get; set; }

    public async Task JoinCart()
    {
        if (!string.IsNullOrEmpty(ViewModel.CartHostEmail))
        {
            AuthenticationState authState = await AuthenticationStateTask;
            if (ViewModel.CartHostEmail == authState.User.Identity.Name)
            {
                ModalViewModel.Header = "Error";
                ModalViewModel.Message = "Can't join your own cart.";
                return;
            }
            ViewModel.IsPolling = true;
            await ViewModel.CartHub.StartAsync();
            HubResponse response = await ViewModel.CartHub.InvokeAsync<HubResponse>(nameof(ICartHubActions.JoinGroup), ViewModel.CartHostEmail);

            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                ViewModel.IsPolling = false;
                await ViewModel.CartHub.StopAsync();
                ViewModel.ShareCartInfo = response.ErrorMessage;
            }
            else if (!string.IsNullOrEmpty(response.SuccessMessage))
            {
                ViewModel.ShareCartInfo = response.SuccessMessage;
            }
        }
        else
        {
            ViewModel.ShareCartInfo = "Give cart host username/email.";
        }
    }

    public async Task ExitCart()
    {
        try
        {
            HubResponse response = await ViewModel.CartHub.InvokeAsync<HubResponse>(nameof(ICartHubActions.LeaveGroup));
            ViewModel.ShareCartInfo = response.ErrorMessage;
        }
        catch (Exception ex)
        {
            ViewModel.ShareCartInfo = ex.Message;
        }
        finally
        {
            await ViewModel.CartHub.StopAsync();
            ViewModel.IsPolling = false;
        }
    }
}

using GroceryListHelper.Client.HelperMethods;
using GroceryListHelper.Client.ViewModels;
using GroceryListHelper.Shared.Interfaces;
using GroceryListHelper.Shared.Models.Authentication;
using GroceryListHelper.Shared.Models.BaseModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;

namespace GroceryListHelper.Client.Components;

public class ShareSelfCartComponentBase : BasePage<IndexViewModel>
{
    [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }
    [Inject] public ModalViewModel ModalViewModel { get; set; }
    public EmailModel AllowEmail { get; set; } = new EmailModel();

    public async Task AddUser()
    {
        AuthenticationState authState = await AuthenticationStateTask;
        if (AllowEmail.Email == authState.User.Identity.Name)
        {
            ModalViewModel.Header = "Error";
            ModalViewModel.Message = "Can't share cart with yourself.";
            return;
        }
        ViewModel.AllowedUsers.Add(AllowEmail.Email);
        AllowEmail = new EmailModel();
    }

    public void DeleteUser(string user)
    {
        ViewModel.AllowedUsers.Remove(user);
    }

    public async Task ShareCart()
    {
        try
        {
            if (ViewModel.AllowedUsers.Count > 0)
            {
                ViewModel.IsPolling = true;
                await ViewModel.CartHub.StartAsync();
                HubResponse response = await ViewModel.CartHub.InvokeAsync<HubResponse>(nameof(ICartHubActions.CreateGroup), ViewModel.AllowedUsers);

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
                ViewModel.ShareCartInfo = "There are no allowed users for your cart.";
            }
        }
        catch (Exception ex)
        {
            ViewModel.ShareCartInfo = ex.Message;
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
            ViewModel.AllowedUsers.Clear();
        }
    }
}

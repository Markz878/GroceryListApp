using GroceryListHelper.Client.HelperMethods;
using GroceryListHelper.Client.ViewModels;
using GroceryListHelper.Shared;
using GroceryListHelper.Shared.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace GroceryListHelper.Client.Components;

public class JoinCartComponentBase : BasePage<IndexViewModel>
{
    public async Task JoinCart()
    {
        if (!string.IsNullOrEmpty(ViewModel.CartHostEmail))
        {
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

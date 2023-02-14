using Microsoft.AspNetCore.Authorization;

namespace GroceryListHelper.Client.Pages;

[Authorize]
public abstract class GroupCartBase : BasePage<MainViewModel>
{
    [Parameter] public Guid GroupId { get; set; }
    [Inject] public required ICartHubClient CartHubClient { get; set; }

    public async Task JoinCart()
    {
        ViewModel.IsPolling = true;
        await CartHubClient.Start();
        HubResponse response = await CartHubClient.JoinGroup(GroupId);
        if (!string.IsNullOrEmpty(response.ErrorMessage))
        {
            ViewModel.IsPolling = false;
            await CartHubClient.Stop();
            ViewModel.ShareCartInfo = response.ErrorMessage;
        }
        else if (!string.IsNullOrEmpty(response.SuccessMessage))
        {
            ViewModel.ShareCartInfo = response.SuccessMessage;
        }
    }

    public async Task ExitCart()
    {
        try
        {
            HubResponse response = await CartHubClient.LeaveGroup(GroupId);
            ViewModel.ShareCartInfo = response.ErrorMessage;
        }
        catch (Exception ex)
        {
            ViewModel.ShareCartInfo = ex.Message;
        }
        finally
        {
            await CartHubClient.Stop();
            ViewModel.IsPolling = false;
        }
    }
}
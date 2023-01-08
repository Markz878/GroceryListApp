namespace GroceryListHelper.Client.Components;

public abstract class JoinCartComponentBase : BasePage<IndexViewModel>
{
    [Inject] public AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
    [Inject] public ModalViewModel ModalViewModel { get; set; } = default!;

    public async Task JoinCart()
    {
        if (!string.IsNullOrEmpty(ViewModel.CartHostEmail))
        {
            AuthenticationState authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            if (ViewModel.CartHostEmail == authState.User?.Identity?.Name)
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
            ViewModel.ShareCartInfo = "Give cart host email.";
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

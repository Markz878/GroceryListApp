namespace GroceryListHelper.Client.Components;

public abstract class ShareSelfCartComponentBase : BasePage<IndexViewModel>
{
    [Inject] public AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
    [Inject] public ModalViewModel ModalViewModel { get; set; } = default!;
    [Inject] public required ICartHubClient CartHubClient { get; set; }
    public EmailModel AllowEmail { get; set; } = new EmailModel();

    public async Task AddUser()
    {
        AuthenticationState authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        if (AllowEmail.Email == authState.User.Identity?.Name)
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
                await CartHubClient.Start();
                HubResponse response = await CartHubClient.CreateGroup(ViewModel.AllowedUsers.ToList());

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
            HubResponse response = await CartHubClient.LeaveGroup();
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
            ViewModel.AllowedUsers.Clear();
        }
    }
}

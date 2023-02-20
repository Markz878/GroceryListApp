using GroceryListHelper.Shared.Models.CartGroups;

namespace GroceryListHelper.Client.Pages;

public abstract class GroupCartBase : BasePage<MainViewModel>
{
    [Parameter] public Guid GroupId { get; set; }
    [Inject] public required ICartHubClient CartHubClient { get; set; }
    [Inject] public required ICartGroupsService CartGroupsService { get; set; }
    [Inject] public required PersistentComponentState ApplicationState { get; set; }

    protected CartGroup? group;
    private PersistingComponentStateSubscription stateSubscription;

    protected override async Task OnInitializedAsync()
    {
        if (!ApplicationState.TryTakeFromJson(nameof(group), out group))
        {
            group = await CartGroupsService.GetCartGroup(GroupId);
        }
        stateSubscription = ApplicationState.RegisterOnPersisting(PersistData);
    }

    private Task PersistData()
    {
        ApplicationState?.PersistAsJson(nameof(group), group);
        return Task.CompletedTask;
    }

    protected async Task<bool> CheckAccess()
    {
        CartGroup? x = await CartGroupsService.GetCartGroup(GroupId);
        return x != null;
    }

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

    public override void Dispose()
    {
        stateSubscription.Dispose();
        base.Dispose();
    }
}
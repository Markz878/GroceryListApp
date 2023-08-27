using GroceryListHelper.Client.Components;

namespace GroceryListHelper.Client.Pages;

public abstract class GroupCartBase : BasePage<MainViewModel>
{
    [Parameter] public Guid GroupId { get; set; }
    [Inject] public required ICartHubClient CartHubClient { get; set; }
    [Inject] public required ICartGroupsService CartGroupsService { get; set; }
    [Inject] public required PersistentComponentState ApplicationState { get; set; }

    protected CartGroup? group;
    private PersistingComponentStateSubscription stateSubscription;
    protected CartComponent? cartComponent;

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
        CartGroup? cartGroup = await CartGroupsService.GetCartGroup(GroupId);
        return cartGroup != null;
    }

    public async Task JoinCart()
    {
        try
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
            else if (group is not null)
            {
                ViewModel.ShareCartInfo = $"You have joined cart {group.Name}.";
                ArgumentNullException.ThrowIfNull(cartComponent);
                await cartComponent.RefreshCartProductsService();
            }
        }
        catch (Exception ex)
        {
            ViewModel.ShareCartInfo = $"Problem connecting: {ex.Message}";
            ViewModel.IsPolling = false;
        }
    }

    public async Task ExitCart()
    {
        try
        {
            HubResponse response = await CartHubClient.LeaveGroup(GroupId);
            ViewModel.ShareCartInfo = response.ErrorMessage;
            ViewModel.IsPolling = false;
            ArgumentNullException.ThrowIfNull(cartComponent);
            await cartComponent.RefreshCartProductsService();
        }
        catch (Exception ex)
        {
            ViewModel.ShareCartInfo = ex.Message;
        }
        finally
        {
            await CartHubClient.Stop();
        }
    }

    public override async ValueTask DisposeAsync()
    {
        if (ViewModel.IsPolling)
        {
            ViewModel.IsPolling = false; // This needs to be set before any async calls
            await ExitCart();
        }
        stateSubscription.Dispose();
        await base.DisposeAsync();
    }
}
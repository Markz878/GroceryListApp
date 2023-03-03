using GroceryListHelper.Client.Components;
using GroceryListHelper.Client.Models;
using GroceryListHelper.Shared.Models.CartGroups;
using GroceryListHelper.Shared.Models.HelperModels;

namespace GroceryListHelper.Client.Pages;

public abstract class ManageGroupsBase : BasePage<MainViewModel>
{
    [Inject] public required ICartGroupsService GroupsService { get; set; }
    [Inject] public required NavigationManager Navigation { get; set; }
    [Inject] public required ModalViewModel Modal { get; set; }
    [Inject] public required PersistentComponentState ApplicationState { get; set; }
    [Inject] public required IJSRuntime JS { get; set; }

    protected List<CartGroup>? cartGroups = new();
    protected bool isCreatingNewGroup;
    protected EmailModel newMemberEmail = new();
    protected CreateCartGroupRequest createCartGroupRequest = new();
    protected CartGroup? isEditingGroup;
    protected CartGroup? isDeletingGroup;
    protected Confirm? confirm;
    private string oldName = "";
    private readonly EmailModelValidator emailValidator = new();
    private readonly CreateCartGroupRequestValidator cartGroupValidator = new();
    private PersistingComponentStateSubscription stateSubscription;

    protected override async Task OnInitializedAsync()
    {
        if (!ApplicationState.TryTakeFromJson(nameof(cartGroups), out cartGroups))
        {
            cartGroups = await GroupsService.GetCartGroups();
        }
        stateSubscription = ApplicationState.RegisterOnPersisting(PersistData);
    }

    private Task PersistData()
    {
        ApplicationState?.PersistAsJson(nameof(cartGroups), cartGroups);
        return Task.CompletedTask;
    }

    protected async Task CreateGroup()
    {
        if (isCreatingNewGroup && cartGroups is not null)
        {
            ValidationResult validationResult = cartGroupValidator.Validate(createCartGroupRequest);
            if (validationResult.IsValid is false)
            {
                Modal.Header = "Error";
                Modal.Message = validationResult.Errors.First().ErrorMessage;
                return;
            }
            Result<CartGroup, UserNotFoundException> response = await GroupsService.CreateCartGroup(createCartGroupRequest);
            response.Match(x =>
            {
                cartGroups.Add(x);
                isCreatingNewGroup = false;
                createCartGroupRequest = new();
            },
            e =>
            {
                Modal.Header = "Error";
                Modal.Message = e.Message;
            });
        }
        else
        {
            isCreatingNewGroup = true;
        }
    }

    protected void SelectCart(Guid groupId)
    {
        Navigation.NavigateTo($"groupcart/{groupId}");
    }

    protected void AddMember()
    {
        ValidationResult validationResult = emailValidator.Validate(newMemberEmail);
        if (validationResult.IsValid is false)
        {
            Modal.Header = "Error";
            Modal.Message = validationResult.Errors.First().ErrorMessage;
            return;
        }
        createCartGroupRequest.OtherUsers.Add(newMemberEmail.Email);
        newMemberEmail = new();
    }

    protected void RemoveNewMember(string member)
    {
        createCartGroupRequest.OtherUsers.Remove(member);
    }

    protected void EditGroup(CartGroup group)
    {
        oldName = group.Name;
        isEditingGroup = group;
    }

    protected async void SubmitEditGroup(CartGroup group)
    {
        isEditingGroup = null;
        if (group.Name != oldName)
        {
            await GroupsService.UpdateCartGroup(new UpdateCartGroupNameRequest() { GroupId = group.Id, Name = group.Name });
        }
        oldName = "";
    }

    protected void StopEditGroup()
    {
        isEditingGroup = null;
        oldName = "";
    }

    protected async Task ShowDeleteConfirm(CartGroup group)
    {
        isDeletingGroup = group;
        if (confirm is not null)
        {
            await confirm.ShowConfirm();
        }
    }

    protected void CancelGroupDelete()
    {
        isDeletingGroup = null;
    }

    protected async Task DeleteGroup()
    {
        ArgumentNullException.ThrowIfNull(isDeletingGroup);
        await GroupsService.DeleteCartGroup(isDeletingGroup.Id);
        cartGroups?.Remove(isDeletingGroup);
    }

    public override void Dispose()
    {
        stateSubscription.Dispose();
        base.Dispose();
    }
}
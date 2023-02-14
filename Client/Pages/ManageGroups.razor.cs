using GroceryListHelper.Client.Models;
using GroceryListHelper.Shared.Models.CartGroups;
using Microsoft.AspNetCore.Authorization;

namespace GroceryListHelper.Client.Pages;

[Authorize]
public abstract class ManageGroupsBase : BasePage<MainViewModel>, IAsyncDisposable
{
    [Inject] public required ICartGroupsService GroupsService { get; set; }
    [Inject] public required NavigationManager Navigation { get; set; }
    [Inject] public required ModalViewModel Modal { get; set; }

    protected List<CartGroup> cartGroups = new();
    protected bool isCreatingNewGroup;
    protected EmailModel newMemberEmail = new();
    protected CreateCartGroupRequest createCartGroupRequest = new();
    protected CartGroup? isEditingGroup;
    private readonly EmailModelValidator emailValidator = new();
    private readonly CreateCartGroupRequestValidator cartGroupValidator = new();

    protected override async Task OnInitializedAsync()
    {
        cartGroups = await GroupsService.GetCartGroups();
    }

    protected async Task CreateGroup()
    {
        if (isCreatingNewGroup)
        {
            ValidationResult validationResult = cartGroupValidator.Validate(createCartGroupRequest);
            if (validationResult.IsValid is false)
            {
                Modal.Header = "Error";
                Modal.Message = validationResult.Errors.First().ErrorMessage;
                return;
            }
            CartGroup? group = await GroupsService.CreateCartGroup(createCartGroupRequest);
            if (group is null)
            {
                Modal.Header = "Error";
                Modal.Message = "Group contained an invalid email, check them again.";
                return;
            }
            cartGroups.Add(group);
            isCreatingNewGroup = false;
            createCartGroupRequest = new();
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
        isEditingGroup = group;
    }

    protected async void SubmitEditGroup(CartGroup group)
    {
        await GroupsService.UpdateCartGroup(new CartGroup() { Id = group.Id, Name = group.Name, OtherUsers = group.OtherUsers });
    }

    protected void StopEditGroup()
    {
        isEditingGroup = null;
    }

    protected async Task LeaveGroup(Guid groupId)
    {
        await GroupsService.LeaveCartGroup(groupId);
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
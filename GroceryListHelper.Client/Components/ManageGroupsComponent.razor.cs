using FluentValidation.Results;
using GroceryListHelper.Client.Features.ManageGroups;
using GroceryListHelper.Shared.Models.CartGroups;
using Microsoft.AspNetCore.Authorization;

namespace GroceryListHelper.Client.Components;

[Authorize]
public partial class ManageGroupsComponent
{
    [Inject] public required AppState AppState { get; init; }
    [Parameter][EditorRequired] public required List<CartGroup> CartGroups { get; init; }
    [Inject] public required IMediator Mediator { get; set; }

    private bool isCreatingNewGroup;
    private readonly EmailModelValidator emailValidator = new();
    private CreateCartGroupRequest createCartGroupRequest = new();
    private readonly CreateCartGroupRequestValidator cartGroupValidator = new();
    private EmailModel newMemberEmail = new();
    private bool isBusy;
    private string oldName = "";
    private CartGroup? isEditingGroup;
    private CartGroup? isDeletingGroup;
    private Confirm? confirm;

    private void AddMember()
    {
        ValidationResult validationResult = emailValidator.Validate(newMemberEmail);
        if (validationResult.IsValid is false)
        {
            AppState.ShowError(validationResult.Errors.First().ErrorMessage);
            return;
        }
        createCartGroupRequest.OtherUsers.Add(newMemberEmail.Email);
        newMemberEmail = new();
    }

    private async Task CreateGroup()
    {
        if (isCreatingNewGroup && CartGroups is not null)
        {
            ValidationResult validationResult = cartGroupValidator.Validate(createCartGroupRequest);
            if (validationResult.IsValid is false)
            {
                AppState.ShowError(validationResult.Errors.First().ErrorMessage);
                return;
            }
            try
            {
                isBusy = true;
                Result<Guid, string> response = await Mediator.Send(new CreateGroupClientCommand() { Request = createCartGroupRequest });
                response.Handle(id =>
                {
                    CartGroup cartGroup = new() { Id = id, Name = createCartGroupRequest.Name, OtherUsers = createCartGroupRequest.OtherUsers.ToHashSet() };
                    CartGroups.Add(cartGroup);
                    createCartGroupRequest = new();
                    isCreatingNewGroup = false;
                }, AppState.ShowError);
            }
            catch (Exception ex)
            {
                AppState.ShowError(ex.Message);
            }
            finally
            {
                isBusy = false;
            }
        }
        else
        {
            isCreatingNewGroup = true;
        }
    }

    private void RemoveNewMember(string member)
    {
        createCartGroupRequest.OtherUsers.Remove(member);
    }

    private void EditGroup(CartGroup group)
    {
        oldName = group.Name;
        isEditingGroup = group;
    }

    private async void SubmitEditGroup(CartGroup group)
    {
        isEditingGroup = null;
        if (group.Name != oldName)
        {
            try
            {
                UpdateGroupNameClientCommandResponse response = await Mediator.Send(new UpdateGroupNameClientCommand() { GroupId = group.Id, GroupName = group.Name });
                if (!string.IsNullOrEmpty(response.Error))
                {
                    AppState.ShowError(response.Error);
                }
            }
            catch (Exception ex)
            {
                AppState.ShowError(ex.Message);
            }
        }
        oldName = "";
    }

    private void StopEditGroup()
    {
        isEditingGroup = null;
        oldName = "";
    }

    private async Task ShowDeleteConfirm(CartGroup group)
    {
        isDeletingGroup = group;
        if (confirm is not null)
        {
            await confirm.ShowConfirm();
        }
    }

    private void CancelGroupDelete()
    {
        isDeletingGroup = null;
    }

    private async Task DeleteGroup()
    {
        if (isDeletingGroup is not null)
        {
            try
            {
                DeleteGroupClientCommandResponse response = await Mediator.Send(new DeleteGroupClientCommand() { GroupId = isDeletingGroup.Id });
                if (!string.IsNullOrEmpty(response.Error))
                {
                    AppState.ShowError(response.Error);
                }
            }
            catch (Exception ex)
            {
                AppState.ShowError(ex.Message);
            }
            CartGroups.Remove(isDeletingGroup);
        }
    }
}
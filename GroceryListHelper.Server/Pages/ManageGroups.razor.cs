using GroceryListHelper.Core.Features.CartGroups;

namespace GroceryListHelper.Server.Pages;

[Authorize]
public partial class ManageGroups
{
    [Inject] public required IMediator Mediator { get; set; }
    [CascadingParameter] public required Task<AuthenticationState> AuthenticationStateTask { get; init; }

    private List<CartGroup>? _cartGroups;

    protected override async Task OnInitializedAsync()
    {
        UserInfo userInfo = await AuthenticationStateTask.GetUserInfo();
        if (userInfo.IsAuthenticated)
        {
            _cartGroups = await Mediator.Send(new GetUserCartGroupsQuery() { UserEmail = userInfo.GetUserEmail() ?? "" });
        }
    }
}
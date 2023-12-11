using GroceryListHelper.Core.Features.CartGroups;
using GroceryListHelper.Shared.Models.HelperModels;

namespace GroceryListHelper.Server.Pages;

public sealed partial class GroupCart
{
    [Parameter] public Guid GroupId { get; set; }
    [Inject] public required IMediator Mediator { get; set; }
    [CascadingParameter] public required Task<AuthenticationState> AuthenticationStateTask { get; init; }

    private List<CartProductCollectable>? cartProducts;
    private List<StoreProduct>? storeProducts;
    private CartGroup? group;
    private string? error;

    protected override async Task OnInitializedAsync()
    {
        UserInfo userInfo = await AuthenticationStateTask.GetUserInfo();
        cartProducts = await Mediator.Send(new GetUserCartProductsQuery() { UserId = GroupId });
        storeProducts = await Mediator.Send(new GetUserStoreProductsQuery() { UserId = userInfo.GetUserId().GetValueOrDefault() });
        Result<CartGroup, ForbiddenError, NotFoundError> getGroupResult = await Mediator.Send(new GetCartGroupQuery() { GroupId = GroupId, UserEmail = userInfo.GetUserEmail() ?? "" });
        getGroupResult.Handle(
            x => group = x,
            f => error = error = "You are not a member of this group",
            n => error = "No group found on this address"
            );
    }
}
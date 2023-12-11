namespace GroceryListHelper.Server.Pages;

public partial class Index
{
    [Inject] public required IMediator Mediator { get; set; }
    [CascadingParameter] public required Task<AuthenticationState> AuthenticationStateTask { get; init; }

    private UserInfo? _userInfo;
    private List<CartProductCollectable>? _cartProducts;
    private List<StoreProduct>? _storeProducts;

    protected override async Task OnInitializedAsync()
    {
        _userInfo = await AuthenticationStateTask.GetUserInfo();
        if (_userInfo.IsAuthenticated)
        {
            _cartProducts = await Mediator.Send(new GetUserCartProductsQuery() { UserId = _userInfo.GetUserId().GetValueOrDefault() });
            _storeProducts = await Mediator.Send(new GetUserStoreProductsQuery() { UserId = _userInfo.GetUserId().GetValueOrDefault() });
        }
        else
        {
            _cartProducts = [];
            _storeProducts = [];
        }
    }
}

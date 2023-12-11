namespace GroceryListHelper.Client.Components;

public partial class CartSummaryRowComponent
{
    [Parameter][EditorRequired] public required ICartProductsService CartProductsService { get; init; }
    [Parameter][EditorRequired] public required IStoreProductsService StoreProductsService { get; init; }
    [CascadingParameter] public required AppState AppState { get; init; }
    public bool AllCollected => AppState.CartProducts.Count > 0 && AppState.CartProducts.All(x => x.IsCollected);
    public double Total => AppState.CartProducts.Sum(x => x.UnitPrice * x.Amount);

    protected string confirmMessage = "";
    protected Func<Task> confirmCallback = () => Task.CompletedTask;
    protected Confirm? confirmDeleteRef;

    public async Task ShowDeleteConfirm(string confirmMessage, Func<Task> confirmCallback)
    {
        this.confirmMessage = confirmMessage;
        this.confirmCallback = confirmCallback;
        if (confirmDeleteRef is not null)
        {
            await confirmDeleteRef.ShowConfirm();
        }
    }

    public async Task ClearCartProducts()
    {
        AppState.CartProducts.Clear();
        await CartProductsService.DeleteAllCartProducts();
    }

    public async Task ClearStoreProducts()
    {
        AppState.StoreProducts.Clear();
        await StoreProductsService.ClearStoreProducts();
    }
}

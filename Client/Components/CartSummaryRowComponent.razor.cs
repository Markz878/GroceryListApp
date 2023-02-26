namespace GroceryListHelper.Client.Components;

public abstract class CartSummaryRowComponentBase : BasePage<MainViewModel>
{
    [Inject] public required ICartProductsService CartProductsService { get; set; }
    [Inject] public required IStoreProductsService StoreProductsService { get; set; }
    public bool AllCollected => ViewModel.CartProducts.Count > 0 && ViewModel.CartProducts.All(x => x.IsCollected);
    public double Total => ViewModel.CartProducts.Sum(x => x.Total);

    public Task ClearCartProducts()
    {
        ViewModel.CartProducts.Clear();
        return CartProductsService.DeleteAllCartProducts();
    }

    public Task ClearStoreProducts()
    {
        ViewModel.StoreProducts.Clear();
        return StoreProductsService.ClearStoreProducts();
    }
}

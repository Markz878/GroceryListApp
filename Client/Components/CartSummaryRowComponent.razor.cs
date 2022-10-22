namespace GroceryListHelper.Client.Components;

public class CartSummaryRowComponentBase : BasePage<IndexViewModel>
{
    [Inject] public ICartProductsService CartProductsService { get; set; } = default!;
    [Inject] public IStoreProductsService StoreProductsService { get; set; } = default!;
    public bool AllCollected => ViewModel.CartProducts.All(x => x.IsCollected);
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

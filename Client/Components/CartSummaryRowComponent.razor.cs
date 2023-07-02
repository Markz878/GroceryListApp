namespace GroceryListHelper.Client.Components;

public abstract class CartSummaryRowComponentBase : BasePage<MainViewModel>
{
    [Inject] public required ICartProductsServiceFactory CartProductsServiceFactory { get; set; }
    [Inject] public required IStoreProductsServiceFactory StoreProductsServiceFactory { get; set; }
    public bool AllCollected => ViewModel.CartProducts.Count > 0 && ViewModel.CartProducts.All(x => x.IsCollected);
    public double Total => ViewModel.CartProducts.Sum(x => x.Total);

    public async Task ClearCartProducts()
    {
        ViewModel.CartProducts.Clear();
        ICartProductsService cartProductsService = await CartProductsServiceFactory.GetCartProductsService();
        await cartProductsService.DeleteAllCartProducts();
    }

    public async Task ClearStoreProducts()
    {
        ViewModel.StoreProducts.Clear();
        IStoreProductsService storeProductsService = await StoreProductsServiceFactory.GetStoreProductsService();
        await storeProductsService.ClearStoreProducts();
    }
}

namespace GroceryListHelper.Client.Services;

public class CartProductsLocalService : ICartProductsService
{
    private readonly ILocalStorageService localStorage;
    private const string cartProductsKey = "cartProducts";

    public CartProductsLocalService(ILocalStorageService localStorage)
    {
        this.localStorage = localStorage;
    }

    public async Task<List<CartProductUIModel>> GetCartProducts()
    {
        return await localStorage.GetItemAsync<List<CartProductUIModel>>(cartProductsKey) ?? new List<CartProductUIModel>();
    }

    public async Task<Guid> SaveCartProduct(CartProduct product)
    {
        List<CartProductUIModel> products = await localStorage.GetItemAsync<List<CartProductUIModel>>(cartProductsKey) ?? new List<CartProductUIModel>();
        CartProductUIModel newProduct = new()
        {
            Amount = product.Amount,
            Name = product.Name,
            Order = product.Order,
            UnitPrice = product.UnitPrice
        };
        products.Add(newProduct);
        await localStorage.SetItemAsync(cartProductsKey, products);
        return newProduct.Id;
    }

    public async Task DeleteCartProduct(Guid id)
    {
        List<CartProductUIModel> products = await localStorage.GetItemAsync<List<CartProductUIModel>>(cartProductsKey);
        products.Remove(products.Find(x => x.Id == id));
        await localStorage.SetItemAsync(cartProductsKey, products);
    }

    public async Task DeleteAllCartProducts()
    {
        await localStorage.RemoveItemAsync(cartProductsKey);
    }

    public async Task UpdateCartProduct(CartProductUIModel cartProduct)
    {
        List<CartProductUIModel> products = await localStorage.GetItemAsync<List<CartProductUIModel>>(cartProductsKey);
        CartProductUIModel product = products.Find(x => x.Id == cartProduct.Id);
        product.Name = cartProduct.Name;
        product.Amount = cartProduct.Amount;
        product.UnitPrice = cartProduct.UnitPrice;
        product.IsCollected = cartProduct.IsCollected;
        product.Order = cartProduct.Order;
        await localStorage.SetItemAsync(cartProductsKey, products);
    }
}

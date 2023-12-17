namespace GroceryListHelper.Client.Interfaces;

public interface ICartProductsService
{
    Task<List<CartProductCollectable>> GetCartProducts();
    Task DeleteAllCartProducts();
    Task DeleteCartProduct(string name);
    Task CreateCartProduct(CartProduct product);
    Task UpdateCartProduct(CartProductCollectable cartProduct);
    Task SortCartProducts(ListSortDirection sortDirection);
}

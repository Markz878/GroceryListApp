using GroceryListHelper.Client.Models;
using GroceryListHelper.Shared.Models.CartProduct;

namespace GroceryListHelper.Client.Services;

public interface ICartProductsService
{
    Task<List<CartProductUIModel>> GetCartProducts();
    Task DeleteAllCartProducts();
    Task DeleteCartProduct(Guid id);
    Task<Guid> SaveCartProduct(CartProduct product);
    Task UpdateCartProduct(CartProductUIModel cartProduct);
}
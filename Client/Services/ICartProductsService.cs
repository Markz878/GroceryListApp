using GroceryListHelper.Client.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GroceryListHelper.Client.Services;

public interface ICartProductsService
{
    Task<List<CartProductUIModel>> GetCartProducts();
    Task<bool> ClearCartProducts();
    Task<bool> DeleteCartProduct(int id);
    Task<bool> MarkCartProductCollected(int id);
    Task<bool> SaveCartProduct(CartProductUIModel product);
    Task<bool> UpdateCartProduct(CartProductUIModel cartProduct);
}
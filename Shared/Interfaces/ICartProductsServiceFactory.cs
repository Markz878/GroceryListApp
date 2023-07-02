namespace GroceryListHelper.Shared.Interfaces;

public interface ICartProductsServiceFactory
{
    Task<ICartProductsService> GetCartProductsService();
}
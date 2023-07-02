namespace GroceryListHelper.Shared.Interfaces;

public interface IStoreProductsServiceFactory
{
    Task<IStoreProductsService> GetStoreProductsService();

}
﻿

using GroceryListHelper.Shared.Interfaces;
using Mapster;

namespace GroceryListHelper.Server.Services;

public class HostStoreProductsServiceProvider : IStoreProductsService
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IStoreProductRepository storeProductRepository;

    public HostStoreProductsServiceProvider(IHttpContextAccessor httpContextAccessor, IStoreProductRepository storeProductRepository)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.storeProductRepository = storeProductRepository;
    }
    public Task<bool> ClearStoreProducts()
    {
        throw new NotImplementedException();
    }

    public async Task<List<StoreProductUIModel>> GetStoreProducts()
    {
        Guid? userName = httpContextAccessor.HttpContext?.User.GetUserId();
        if (userName == null)
        {
            return new List<StoreProductUIModel>();
        }
        List<StoreProductServerModel> storeProducts = await storeProductRepository.GetStoreProductsForUser(userName.Value);
        return storeProducts.Select(x => x.Adapt<StoreProductUIModel>()).ToList();
    }

    public Task<string> SaveStoreProduct(StoreProductModel product)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateStoreProduct(StoreProductUIModel storeProduct)
    {
        throw new NotImplementedException();
    }
}
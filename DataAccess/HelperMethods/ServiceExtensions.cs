using Azure.Data.Tables;
using GroceryListHelper.DataAccess.Models;
using GroceryListHelper.DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace GroceryListHelper.DataAccess.HelperMethods;

public static class ServiceExtensions
{
    public static void AddDataAccessServices(this IServiceCollection services)
    {
        services.AddScoped<ICartProductRepository, CartProductRepository>();
        services.AddScoped<IStoreProductRepository, StoreProductRepository>();
        services.AddScoped<ICartGroupRepository, CartGroupRepository>();

    }

    public static void InitDatabase(this IServiceProvider services)
    {
        TableServiceClient tableService = services.GetRequiredService<TableServiceClient>();
        tableService.CreateTableIfNotExists(CartProductDbModel.GetTableName());
        tableService.CreateTableIfNotExists(StoreProductDbModel.GetTableName());
        tableService.CreateTableIfNotExists(CartGroupUserDbModel.GetTableName());
        tableService.CreateTableIfNotExists(CartUserGroupDbModel.GetTableName());
        tableService.CreateTableIfNotExists(ActiveCartGroupDbModel.GetTableName());
    }
}

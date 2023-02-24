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
        services.AddScoped<IUserRepository, UserRepository>();
    }

    public static void InitDatabase(this IServiceProvider services)
    {
        TableServiceClient tableService = services.GetRequiredService<TableServiceClient>();
        tableService.CreateTableIfNotExists(CartProductDbModel.GetTableName());
        tableService.CreateTableIfNotExists(StoreProductDbModel.GetTableName());
        tableService.CreateTableIfNotExists(CartGroupUserDbModel.GetTableName());
        tableService.CreateTableIfNotExists(CartUserGroupDbModel.GetTableName());
        tableService.CreateTableIfNotExists(ActiveCartGroupDbModel.GetTableName());
        tableService.CreateTableIfNotExists(UserDbModel.GetTableName());
    }

    public static void DeleteDatabase(this IServiceProvider services)
    {
        TableServiceClient tableService = services.GetRequiredService<TableServiceClient>();
        if (tableService.AccountName == "devstoreaccount1")
        {
            tableService.DeleteTable(CartProductDbModel.GetTableName());
            tableService.DeleteTable(StoreProductDbModel.GetTableName());
            tableService.DeleteTable(CartGroupUserDbModel.GetTableName());
            tableService.DeleteTable(CartUserGroupDbModel.GetTableName());
            tableService.DeleteTable(ActiveCartGroupDbModel.GetTableName());
            tableService.DeleteTable(UserDbModel.GetTableName());
        }
    }

    public static string[] GetTables()
    {
        return new[]
        {
            CartProductDbModel.GetTableName(),
            StoreProductDbModel.GetTableName(),
            CartGroupUserDbModel.GetTableName(),
            CartUserGroupDbModel.GetTableName(),
            ActiveCartGroupDbModel.GetTableName(),
            UserDbModel.GetTableName()
        };
    }
}

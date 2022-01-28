using GroceryListHelper.DataAccess;
using GroceryListHelper.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GroceryListHelper.Server.Installers;

public class DataAccessInstaller : IInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<GroceryStoreDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DatabaseConnection")));

        services.AddScoped<ICartProductRepository, CartProductRepository>();
        services.AddScoped<IStoreProductRepository, StoreProductRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
    }
}

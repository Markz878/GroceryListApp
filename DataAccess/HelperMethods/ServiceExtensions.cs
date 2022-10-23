using GroceryListHelper.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GroceryListHelper.DataAccess.HelperMethods;

public static class ServiceExtensions
{
    public static void AddDataAccessServices(this IServiceCollection services, IConfiguration configuration, bool addSensitiveLogging)
    {
        services.AddDbContext<GroceryStoreDbContext>(options =>
        {
            options.UseCosmos(configuration.GetConnectionString("Cosmos"), "GroceryListDb");
            options.EnableThreadSafetyChecks(false);
            if (addSensitiveLogging)
            {
                options.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging(true);
            }
        });
        services.AddScoped<ICartProductRepository, CartProductRepository>();
        services.AddScoped<IStoreProductRepository, StoreProductRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
    }

    public static void EnsureDatabaseCreated(this IHost app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        GroceryStoreDbContext db = scope.ServiceProvider.GetRequiredService<GroceryStoreDbContext>();
        db.Database.EnsureCreated();
    }
}

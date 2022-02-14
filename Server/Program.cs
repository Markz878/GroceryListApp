using GroceryListHelper.DataAccess;

namespace GroceryListHelper.Server;

public class Program
{
    public static void Main(string[] args)
    {
        IHost host = CreateHostBuilder(args).Build();
        //IServiceScopeFactory scopeFactory = host.Services.GetRequiredService<IServiceScopeFactory>();
        //using (IServiceScope scope = scopeFactory.CreateScope())
        //{
        //    ILogger<Program> logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        //    GroceryStoreDbContext db = scope.ServiceProvider.GetRequiredService<GroceryStoreDbContext>();
        //    try
        //    {
        //        db.Database.EnsureCreated();
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError(ex, ex.Message);
        //        logger.LogError(Environment.CurrentDirectory);
        //    }
        //}
        host.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
    }
}

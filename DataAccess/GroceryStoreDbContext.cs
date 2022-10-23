using GroceryListHelper.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace GroceryListHelper.DataAccess;

public class GroceryStoreDbContext : DbContext
{
    public GroceryStoreDbContext(DbContextOptions<GroceryStoreDbContext> options) : base(options)
    {
    }

    public DbSet<CartProductDbModel> CartProducts { get; set; } = default!;
    public DbSet<StoreProductDbModel> StoreProducts { get; set; } = default!;
    public DbSet<UserCartGroupDbModel> UserCartGroups { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<CartProductDbModel>().ToContainer("CartProducts").HasNoDiscriminator().HasPartitionKey(x => x.UserId).HasDefaultTimeToLive(60 * 60 * 24 * 7).Property(x => x.Id).ToJsonProperty("id");
        builder.Entity<StoreProductDbModel>().ToContainer("StoreProducts").HasNoDiscriminator().HasPartitionKey(x => x.UserId).HasDefaultTimeToLive(60 * 60 * 24 * 60).Property(x => x.Id).ToJsonProperty("id");
        builder.Entity<UserCartGroupDbModel>().ToContainer("UserCartGroups").HasNoDiscriminator().HasPartitionKey(x => x.HostId).HasDefaultTimeToLive(60 * 60).Property(x => x.Id).ToJsonProperty("id");
    }
}

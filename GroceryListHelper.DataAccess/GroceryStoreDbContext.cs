using GroceryListHelper.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace GroceryListHelper.DataAccess
{
    public class GroceryStoreDbContext : DbContext
    {
        public GroceryStoreDbContext(DbContextOptions<GroceryStoreDbContext> options) : base(options)
        {
        }

        public DbSet<UserDbModel> Users { get; set; }
        public DbSet<CartProductDbModel> CartProducts { get; set; }
        public DbSet<StoreProductDbModel> StoreProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserDbModel>(x => x.HasIndex(x => x.Email).IsUnique());
            builder.Entity<CartProductDbModel>(x => x.HasOne<UserDbModel>().WithMany().HasForeignKey(c => c.UserId));
            builder.Entity<StoreProductDbModel>(x => x.HasOne<UserDbModel>().WithMany().HasForeignKey(c => c.UserId));
        }
    }
}

using GroceryListHelper.Shared;

namespace GroceryListHelper.Server.Models
{
    public class StoreProductDbModel : StoreProduct
    {
        public int Id { get; set; }
        public int UserId { get; set; }
    }
}

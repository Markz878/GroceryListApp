using GroceryListHelper.Shared.Models.StoreProduct;

namespace GroceryListHelper.DataAccess.Models;

public record StoreProductDbModel : StoreProductServerModel
{
    public Guid UserId { get; set; }
}

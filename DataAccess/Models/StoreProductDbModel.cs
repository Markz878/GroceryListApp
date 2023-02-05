using GroceryListHelper.Shared.Models.StoreProducts;

namespace GroceryListHelper.DataAccess.Models;

public record StoreProductDbModel : StoreProductServerModel
{
    public Guid UserId { get; set; }
}

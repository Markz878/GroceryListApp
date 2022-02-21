using GroceryListHelper.Shared.Models.StoreProduct;

namespace GroceryListHelper.DataAccess.Models;

public record StoreProductDbModel : StoreProductServerModel
{
    public string UserId { get; set; }
}

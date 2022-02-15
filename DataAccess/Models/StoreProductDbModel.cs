using GroceryListHelper.Shared.Models.StoreProduct;

namespace GroceryListHelper.DataAccess.Models;

public record StoreProductDbModel : StoreProductResponseModel
{
    public string UserId { get; set; }
}

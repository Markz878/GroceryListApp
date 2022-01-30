using GroceryListHelper.Shared.Models.StoreProduct;

namespace GroceryListHelper.DataAccess.Models;

public class StoreProductDbModel : StoreProductResponseModel
{
    public string UserId { get; set; }
}

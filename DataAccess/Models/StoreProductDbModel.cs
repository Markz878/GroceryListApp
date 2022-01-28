using GroceryListHelper.Shared.Models.StoreProduct;

namespace GroceryListHelper.DataAccess.Models;

public class StoreProductDbModel : StoreProductResponseModel
{
    public int UserId { get; set; }
}

using GroceryListHelper.Shared.Models.StoreProduct;

namespace GroceryListHelper.Shared.Models.CartProduct;

public class CartProduct : StoreProductModel
{
    public double Amount { get; set; }
    public double Order { get; set; }
}

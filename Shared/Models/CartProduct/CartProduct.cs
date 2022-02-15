using GroceryListHelper.Shared.Models.StoreProduct;

namespace GroceryListHelper.Shared.Models.CartProduct;

public record CartProduct : StoreProductModel
{
    public double Amount { get; set; } = 1;
    public double Order { get; set; }
}

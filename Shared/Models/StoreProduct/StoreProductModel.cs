namespace GroceryListHelper.Shared.Models.StoreProduct;

public record StoreProductModel
{
    public string Name { get; set; } = string.Empty;
    public double UnitPrice { get; set; }
}

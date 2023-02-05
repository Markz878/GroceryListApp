namespace GroceryListHelper.Shared.Models.StoreProducts;

public record StoreProductUIModel : StoreProduct
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
}

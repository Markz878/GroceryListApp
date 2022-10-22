namespace GroceryListHelper.Shared.Models.StoreProduct;

public record StoreProductUIModel : StoreProductModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
}

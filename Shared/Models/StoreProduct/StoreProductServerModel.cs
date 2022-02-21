namespace GroceryListHelper.Shared.Models.StoreProduct;

public record StoreProductServerModel : StoreProductModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
}

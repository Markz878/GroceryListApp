namespace GroceryListHelper.Shared.Models.StoreProduct;

public record StoreProductResponseModel : StoreProductModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
}

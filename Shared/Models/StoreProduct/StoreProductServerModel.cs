namespace GroceryListHelper.Shared.Models.StoreProduct;

public record StoreProductServerModel : StoreProductModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
}

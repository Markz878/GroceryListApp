namespace GroceryListHelper.Shared.Models.StoreProduct;

public class StoreProductResponseModel : StoreProductModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
}

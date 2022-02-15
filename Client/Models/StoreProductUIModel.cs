using GroceryListHelper.Shared.Models.StoreProduct;

namespace GroceryListHelper.Client.Models;

public record StoreProductUIModel : StoreProductModel
{
    public string Id { get; set; }
}

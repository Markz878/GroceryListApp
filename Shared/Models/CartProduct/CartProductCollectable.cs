using GroceryListHelper.Shared.Models.CartProduct;

namespace GroceryListHelper.Shared;

public record CartProductCollectable : CartProduct
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public bool IsCollected { get; set; }
}

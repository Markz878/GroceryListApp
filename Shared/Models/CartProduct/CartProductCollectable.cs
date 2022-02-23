namespace GroceryListHelper.Shared.Models.CartProduct;

public record CartProductCollectable : CartProduct
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public bool IsCollected { get; set; }
}

namespace GroceryListHelper.Shared.Models.CartProduct;

public record CartProductCollectable : CartProduct
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public bool IsCollected { get; set; }
}

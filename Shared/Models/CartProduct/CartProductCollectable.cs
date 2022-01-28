using GroceryListHelper.Shared.Models.CartProduct;

namespace GroceryListHelper.Shared;

public class CartProductCollectable : CartProduct
{
    public int Id { get; set; }
    public bool IsCollected { get; set; }
}

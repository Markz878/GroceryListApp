namespace GroceryListHelper.Shared.Models.CartProduct;

public record CartProductUIModel : CartProductCollectable
{
    public double Total => UnitPrice * Amount;
}

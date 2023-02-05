namespace GroceryListHelper.Shared.Models.CartProducts;

public record CartProductUIModel : CartProductCollectable
{
    public double Total => UnitPrice * Amount;
}

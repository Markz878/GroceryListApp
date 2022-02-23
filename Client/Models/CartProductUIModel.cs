using GroceryListHelper.Shared.Models.CartProduct;

namespace GroceryListHelper.Client.Models;

public record CartProductUIModel : CartProductCollectable
{
    public double Total => UnitPrice * Amount;
}

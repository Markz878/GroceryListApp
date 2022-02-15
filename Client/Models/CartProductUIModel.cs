using GroceryListHelper.Shared;

namespace GroceryListHelper.Client.Models;

public record CartProductUIModel : CartProductCollectable
{
    public double Total => UnitPrice * Amount;
}

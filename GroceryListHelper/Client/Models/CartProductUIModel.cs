using GroceryListHelper.Shared;

namespace GroceryListHelper.Client.Models
{
    public class CartProductUIModel : CartProductCollectable
    {
        public double Total => UnitPrice * Amount;
    }
}

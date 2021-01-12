using GroceryListHelper.Shared;

namespace GroceryListHelper.Client.Models
{
    public class CartProductUIModel : CartProductCollectable
    {
        public int Id { get; set; }
        public double Total => UnitPrice * Amount;
    }
}

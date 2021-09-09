using GroceryListHelper.Client.HelperMethods;

namespace GroceryListHelper.Client.Models
{
    public class CartProductUIModel
    {
        public int Id { get; set; }
        public bool IsCollected { get; set; }
        public double Total => UnitPrice * Amount;
        public double Amount { get; set; }
        public string Name { get; set; }
        public double UnitPrice { get; set; }
    }
}

using GroceryListHelper.Shared;

namespace GroceryListHelper.Server.Models
{
    public class CartProductDbModel : CartProductCollectable
    {
        public int UserId { get; set; }
    }
}

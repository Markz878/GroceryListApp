using GroceryListHelper.Shared;

namespace GroceryListHelper.Server.Models
{
    public class CartProductDbModel : CartProductCollectable
    {
        public int Id { get; set; }
        public int UserId { get; set; }
    }
}

using GroceryListHelper.Shared;

namespace GroceryListHelper.DataAccess.Models;

public class CartProductDbModel : CartProductCollectable
{
    public int UserId { get; set; }
}

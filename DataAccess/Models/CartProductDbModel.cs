using GroceryListHelper.Shared;

namespace GroceryListHelper.DataAccess.Models;

public class CartProductDbModel : CartProductCollectable
{
    public string UserId { get; set; }
}

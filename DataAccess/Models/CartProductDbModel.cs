using GroceryListHelper.Shared.Models.CartProducts;

namespace GroceryListHelper.DataAccess.Models;

public record CartProductDbModel : CartProductCollectable
{
    public Guid UserId { get; set; }
}

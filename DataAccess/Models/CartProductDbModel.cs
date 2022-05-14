using GroceryListHelper.Shared.Models.CartProduct;

namespace GroceryListHelper.DataAccess.Models;

public record CartProductDbModel : CartProductCollectable
{
    public Guid UserId { get; set; }
}

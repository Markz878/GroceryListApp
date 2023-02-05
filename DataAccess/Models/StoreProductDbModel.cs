using GroceryListHelper.Shared.Models.StoreProducts;

namespace GroceryListHelper.DataAccess.Models;

public record StoreProductDbModel : StoreProductUIModel
{
    public Guid UserId { get; set; }
}

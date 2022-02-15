using GroceryListHelper.Shared;

namespace GroceryListHelper.DataAccess.Models;

public record CartProductDbModel : CartProductCollectable
{
    public string UserId { get; set; }
}

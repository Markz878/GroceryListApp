namespace GroceryListHelper.Shared.Models.CartGroups;

public class CartGroup
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public HashSet<string> OtherUsers { get; set; } = new();
}

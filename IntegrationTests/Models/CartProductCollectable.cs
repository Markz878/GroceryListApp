namespace GroceryListHelper.IntegrationTests.Models;

internal class CartProductCollectable
{
    public string Id { get; set; } = string.Empty;
    public bool IsCollected { get; set; }
    public double Amount { get; set; }
    public string Name { get; set; } = string.Empty;
    public double UnitPrice { get; set; }
}

namespace GroceryListHelper.IntegrationTests.Models;

internal class CartProductCollectable
{
    public string Id { get; set; }
    public bool IsCollected { get; set; }
    public double Amount { get; set; }
    public string Name { get; set; }
    public double UnitPrice { get; set; }
}

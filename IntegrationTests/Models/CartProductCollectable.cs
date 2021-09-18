namespace GroceryListHelper.IntegrationTests.Features
{
    internal class CartProductCollectable
    {
        public int Id { get; set; }
        public bool IsCollected { get; set; }
        public double Amount { get; set; }
        public string Name { get; set; }
        public double UnitPrice { get; set; }
    }
}
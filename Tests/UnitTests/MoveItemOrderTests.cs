using GroceryListHelper.Client.HelperMethods;

namespace GroceryListHelper.Tests.UnitTests;

public sealed class MoveItemOrderTests
{
    [Theory]
    [InlineData(new double[] { 1000, 2000, 3000, 4000, 5000 }, 1000, 3000, 3500)]
    [InlineData(new double[] { 1000, 2000, 3000, 4000, 5000 }, 5000, 3000, 2500)]
    [InlineData(new double[] { 1000, 2000, 3000, 4000, 5000 }, 1000, 5000, 6000)]
    [InlineData(new double[] { 1000, 2000, 3000, 4000, 5000 }, 5000, 1000, 500)]
    public void MoveItemOrderTest(double[] initial, double movingOrder, double movingToOrder, double newOrder)
    {
        double result = SortOrderMethods.GetNewOrder(initial, movingOrder, movingToOrder);
        Assert.Equal(newOrder, result);
    }
}

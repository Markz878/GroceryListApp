using GroceryListHelper.Client.HelperMethods;
using Xunit;

namespace GroceryListHelper.UnitTests.UnitTests;

public class MoveItemOrderTests
{
    [Theory]
    [InlineData(new double[] { 1000, 2000, 3000, 4000, 5000 }, 1000, 3000, new double[] { 2000, 3000, 3500, 4000, 5000 }, 3500)]
    [InlineData(new double[] { 1000, 2000, 3000, 4000, 5000 }, 5000, 3000, new double[] { 1000, 2000, 2500, 3000, 4000 }, 2500)]
    [InlineData(new double[] { 1000, 2000, 3000, 4000, 5000 }, 1000, 5000, new double[] { 2000, 3000, 4000, 5000, 6000 }, 6000)]
    [InlineData(new double[] { 1000, 2000, 3000, 4000, 5000 }, 5000, 1000, new double[] { 500, 1000, 2000, 3000, 4000 }, 500)]
    public void MoveItemOrderTest(double[] initial, double movingOrder, double movingToOrder, double[] newOrders, double newOrder)
    {
        double result = SortOrderMethods.GetNewOrder(initial, movingOrder, movingToOrder);
        Assert.Equal(newOrder, result);
    }
}

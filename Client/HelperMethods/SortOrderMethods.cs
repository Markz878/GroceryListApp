namespace GroceryListHelper.Client.HelperMethods;

public class SortOrderMethods
{
    public static double GetNewOrder(IEnumerable<double> orders, double movingOrder, double movingToOrder)
    {
        double result = -1;
        double minOrder = orders.Min();
        double maxOrder = orders.Max();
        if (movingToOrder == minOrder)
        {
            result = minOrder / 2;
        }
        else if (movingToOrder == maxOrder)
        {
            result = maxOrder + 1000;
        }
        else if (movingOrder < movingToOrder)
        {
            double nextLargestOrder = GetNextLargerOrder(orders.Where(x => x != movingOrder), movingToOrder);
            result = (nextLargestOrder + movingToOrder) / 2;
        }
        else if (movingOrder > movingToOrder)
        {
            double nextLargestOrder = GetNextSmallerOrder(orders.Where(x => x != movingOrder), movingToOrder);
            result = (nextLargestOrder + movingToOrder) / 2;
        }
        if (result == -1)
        {
            throw new InvalidOperationException("Could not get new order");
        }
        return result;
    }

    private static double GetNextLargerOrder(IEnumerable<double> enumerable, double movingToOrder)
    {
        double result = double.MaxValue;
        foreach (double item in enumerable)
        {
            if (item > movingToOrder && item < result)
            {
                result = item;
            }
        }
        return result;
    }

    private static double GetNextSmallerOrder(IEnumerable<double> enumerable, double movingToOrder)
    {
        double result = double.MinValue;
        foreach (double item in enumerable)
        {
            if (item < movingToOrder && item > result)
            {
                result = item;
            }
        }
        return result;
    }
}

namespace GroceryListHelper.Core.HelperMethods;
public static class ProductSortMethods
{
    public static void SortProducts(IEnumerable<CartProduct> cartProducts, ListSortDirection sortDirection)
    {
        int order = 1000;
        if (sortDirection is ListSortDirection.Ascending)
        {
            foreach (CartProduct item in cartProducts.OrderBy(x => x.Name))
            {
                item.Order = order;
                order += 1000;
            }
        }
        else
        {
            foreach (CartProduct item in cartProducts.OrderByDescending(x => x.Name))
            {
                item.Order = order;
                order += 1000;
            }
        }
    }
}

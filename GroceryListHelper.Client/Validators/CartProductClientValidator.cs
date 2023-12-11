namespace GroceryListHelper.Client.Validators;

public sealed class CartProductClientValidator : CartProductValidator
{
    private readonly IEnumerable<CartProductCollectable> productList;

    public CartProductClientValidator(IEnumerable<CartProductCollectable> productList)
    {
        this.productList = productList;
        RuleFor(x => x.Name).Must(BeUnique).WithMessage("Product name must be unique");
    }

    private bool BeUnique(CartProduct product, string name)
    {
        foreach (CartProduct item in productList)
        {
            if (product.Equals(item))
            {
                return product.Name == item.Name;
            }
            else
            {
                if (item.Name == name)
                {
                    return false;
                }
            }
        }
        return true;
    }
}

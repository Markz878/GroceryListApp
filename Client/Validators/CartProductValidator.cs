using FluentValidation;
using GroceryListHelper.Client.Models;
using GroceryListHelper.Shared.Models.CartProduct;

namespace GroceryListHelper.Client.Validators;

public class CartProductValidator : AbstractValidator<CartProduct>
{
    private readonly IEnumerable<CartProductUIModel> productList;

    public CartProductValidator(IEnumerable<CartProductUIModel> productList)
    {
        this.productList = productList;
        RuleFor(x => x.Name).NotEmpty().Must(BeUnique).WithMessage("Product name must be unique");
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
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

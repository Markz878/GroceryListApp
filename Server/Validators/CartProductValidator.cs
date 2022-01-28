using FluentValidation;
using GroceryListHelper.Shared.Models.CartProduct;

namespace GroceryListHelper.Server.Validators;

public class CartProductValidator : AbstractValidator<CartProduct>
{
    public CartProductValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Amount).InclusiveBetween(0, 1e4);
        RuleFor(x => x.UnitPrice).InclusiveBetween(0, 1e4);
    }
}

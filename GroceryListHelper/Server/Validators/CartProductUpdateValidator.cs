using FluentValidation;
using GroceryListHelper.Shared;

namespace GroceryListHelper.Server.Validators
{
    public class CartProductUpdateValidator : AbstractValidator<CartProduct>
    {
        public CartProductUpdateValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(20);
            RuleFor(x => x.Amount).InclusiveBetween(0,1e4);
            RuleFor(x => x.UnitPrice).InclusiveBetween(0, 1e4);
        }
    }
}

using FluentValidation;
using GroceryListHelper.Shared;

namespace GroceryListHelper.Server.Validators
{
    public class StoreProductValidator : AbstractValidator<StoreProduct>
    {
        public StoreProductValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(20);
            RuleFor(x => x.UnitPrice).InclusiveBetween(0, 1e4);
        }
    }
}

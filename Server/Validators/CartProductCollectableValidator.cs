﻿

namespace GroceryListHelper.Server.Validators;

public class CartProductCollectableValidator : AbstractValidator<CartProductCollectable>
{
    public CartProductCollectableValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Amount).InclusiveBetween(0, 1e4);
        RuleFor(x => x.UnitPrice).InclusiveBetween(0, 1e4);
    }
}
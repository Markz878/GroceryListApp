﻿using FluentValidation;
using GroceryListHelper.Shared.Models.StoreProducts;

namespace GroceryListHelper.Shared.Models.CartProducts;

public record CartProduct : StoreProduct
{
    public double Amount { get; set; } = 1;
    public double Order { get; set; }
}

public class CartProductValidator : AbstractValidator<CartProduct>
{
    public CartProductValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Amount).InclusiveBetween(0, 1e4);
        RuleFor(x => x.UnitPrice).InclusiveBetween(0, 1e4);
    }
}
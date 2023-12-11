using FluentValidation;

namespace GroceryListHelper.Shared.Models.StoreProducts;

public record StoreProduct
{
    public string Name { get; set; } = string.Empty;
    public double UnitPrice { get; set; }
}

public class StoreProductValidator : AbstractValidator<StoreProduct>
{
    public StoreProductValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(30);
        RuleFor(x => x.UnitPrice).InclusiveBetween(0, 1e4);
    }
}

using FluentValidation;

namespace GroceryListHelper.Shared.Models.StoreProducts;

public record StoreProductUIModel : StoreProduct
{
    public Guid Id { get; set; } = Guid.NewGuid();
}

public class StoreProductUIModelValidator : AbstractValidator<StoreProductUIModel>
{
    public StoreProductUIModelValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(30);
        RuleFor(x => x.UnitPrice).InclusiveBetween(0, 1e4);
    }
}

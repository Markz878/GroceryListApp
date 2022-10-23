namespace GroceryListHelper.Server.Validators;

public class StoreProductValidator : AbstractValidator<StoreProductModel>
{
    public StoreProductValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(30);
        RuleFor(x => x.UnitPrice).InclusiveBetween(0, 1e4);
    }
}

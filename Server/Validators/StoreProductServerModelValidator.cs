namespace GroceryListHelper.Server.Validators;

public sealed class StoreProductServerModelValidator : AbstractValidator<StoreProductServerModel>
{
    public StoreProductServerModelValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(30);
        RuleFor(x => x.UnitPrice).InclusiveBetween(0, 1e4);
    }
}

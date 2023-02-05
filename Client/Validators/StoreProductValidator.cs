namespace GroceryListHelper.Client.Validators;

public sealed class StoreProductClientValidator : StoreProductValidator
{
    private readonly IEnumerable<StoreProduct> productList;

    public StoreProductClientValidator(IEnumerable<StoreProduct> productList)
    {
        this.productList = productList;
        RuleFor(x => x.Name).Must(BeUnique).WithMessage("Product name must be unique");
    }

    private bool BeUnique(StoreProduct product, string name)
    {
        return productList.All(x => x.Equals(product) || !x.Name.Equals(name));
    }
}

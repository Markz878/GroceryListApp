using FluentValidation;
using GroceryListHelper.Shared.Models.StoreProduct;
using System.Collections.Generic;
using System.Linq;

namespace GroceryListHelper.Client.Validators;

public class StoreProductValidator : AbstractValidator<StoreProductModel>
{
    private readonly IEnumerable<StoreProductModel> productList;

    public StoreProductValidator(IEnumerable<StoreProductModel> productList)
    {
        this.productList = productList;
        RuleFor(x => x.Name).NotEmpty().Must(BeUnique).WithMessage("Product name must be unique");
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
    }

    private bool BeUnique(StoreProductModel product, string name)
    {
        return productList.All(x => x.Equals(product) || !x.Name.Equals(name));
    }
}

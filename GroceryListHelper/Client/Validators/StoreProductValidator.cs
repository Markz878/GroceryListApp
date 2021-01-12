using FluentValidation;
using GroceryListHelper.Shared;
using System.Collections.Generic;
using System.Linq;

namespace GroceryListHelper.Client.Validators
{
    public class StoreProductValidator : AbstractValidator<StoreProduct>
    {
        private readonly IEnumerable<StoreProduct> productList;

        public StoreProductValidator(IEnumerable<StoreProduct> productList)
        {
            this.productList = productList;
            RuleFor(x => x.Name).NotEmpty().Must(BeUnique).WithMessage("Product name must be unique");
            RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
        }

        private bool BeUnique(StoreProduct product, string name)
        {
            return productList.All(x => x.Equals(product) || !x.Name.Equals(name));
        }
    }
}

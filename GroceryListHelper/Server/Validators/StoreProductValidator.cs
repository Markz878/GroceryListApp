using FluentValidation;
using GroceryListHelper.Shared;
using System.Collections.Generic;
using System.Linq;

namespace GroceryListHelper.Server.Validators
{
    public class StoreProductValidator : AbstractValidator<StoreProduct>
    {
        private readonly IEnumerable<StoreProduct> productList;

        public StoreProductValidator(IEnumerable<StoreProduct> productList)
        {
            this.productList = productList;
            RuleFor(x => x.Name).NotEmpty().MaximumLength(20).Must(BeUnique).WithMessage("Product name must be unique");
            RuleFor(x => x.UnitPrice).InclusiveBetween(0, 1e4);
        }

        private bool BeUnique(string name)
        {
            return productList.All(x => !x.Name.Equals(name));
        }
    }
}

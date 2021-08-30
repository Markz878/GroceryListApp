using FluentValidation;
using GroceryListHelper.Shared.Models.CartProduct;
using System.Collections.Generic;

namespace GroceryListHelper.Client.Validators
{
    public class CartProductValidator : AbstractValidator<CartProduct>
    {
        private readonly IEnumerable<CartProduct> productList;

        public CartProductValidator(IEnumerable<CartProduct> productList)
        {
            this.productList = productList;
            RuleFor(x => x.Name).NotEmpty().Must(BeUnique).WithMessage("Product name must be unique");
            RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
            RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
        }

        private bool BeUnique(CartProduct product, string name)
        {
            foreach (var item in productList)
            {
                if (product.Equals(item))
                {
                    return product.Name == item.Name;
                }
                else
                {
                    if (item.Name == name)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}

using FluentValidation;
using GroceryListHelper.Client.Models;
using System.Collections.Generic;

namespace GroceryListHelper.Client.Validators
{
    public class CartProductValidator : AbstractValidator<CartProductUIModel>
    {
        private readonly IEnumerable<CartProductUIModel> productList;

        public CartProductValidator(IEnumerable<CartProductUIModel> productList)
        {
            this.productList = productList;
            RuleFor(x => x.Name).NotEmpty().Must(BeUnique).WithMessage("Product name must be unique");
            RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
            RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
        }

        private bool BeUnique(CartProductUIModel product, string name)
        {
            foreach (CartProductUIModel item in productList)
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

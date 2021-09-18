using FluentValidation;
using GroceryListHelper.Shared.Models.Authentication;
using System.Linq;

namespace GroceryListHelper.Shared.Validators
{
    public class DeleteProfileRequestValidator : AbstractValidator<DeleteProfileRequest>
    {
        public DeleteProfileRequestValidator()
        {
            RuleFor(x => x.Password).Cascade(CascadeMode.Stop).NotEmpty().Length(6, 30).Must(x => x.Any(char.IsDigit)).WithMessage("Password must contain at least one digit");
        }
    }
}

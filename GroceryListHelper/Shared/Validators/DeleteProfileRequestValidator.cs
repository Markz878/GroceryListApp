using FluentValidation;
using System.Linq;

namespace GroceryListHelper.Shared.Validators
{
    public class DeleteProfileRequestValidator : AbstractValidator<DeleteProfileRequest>
    {
        public DeleteProfileRequestValidator()
        {
            RuleFor(x => x.Password).Length(6, 30).Must(x => x.Any(char.IsDigit)).WithMessage("Password must contain at least one digit");
        }
    }
}

using FluentValidation;
using System.Linq;

namespace GroceryListHelper.Shared.Validators
{
    public class ChangeEmailRequestValidator : AbstractValidator<ChangeEmailRequest>
    {
        public ChangeEmailRequestValidator()
        {
            RuleFor(x => x.NewEmail).EmailAddress();
            RuleFor(x => x.Password).NotEmpty().Length(6, 30).Must(x=>x.Any(char.IsDigit)).WithMessage("Password must contain at least one digit");
        }
    }
}

using FluentValidation;
using System.Linq;


namespace GroceryListHelper.Shared.Validators
{
    public class UserCredentialsValidator : AbstractValidator<UserCredentialsModel>
    {
        public UserCredentialsValidator()
        {
            RuleFor(x => x.Email).EmailAddress();
            RuleFor(x => x.Password).Cascade(CascadeMode.Stop).NotEmpty().Length(6, 30).Must(x => x.Any(char.IsDigit)).WithMessage("Password must contain at least one digit");
        }
    }
}

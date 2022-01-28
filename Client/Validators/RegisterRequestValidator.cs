using FluentValidation;
using GroceryListHelper.Shared.Models.Authentication;
using System.Linq;

namespace GroceryListHelper.Client.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequestModel>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x => x.Password).Cascade(CascadeMode.Stop).NotEmpty().Length(6, 30).Must(x => x.Any(char.IsDigit)).WithMessage("Password must contain at least one digit");
        RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Confirm password should be the same as password.");
    }
}

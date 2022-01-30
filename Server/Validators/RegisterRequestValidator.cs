using FluentValidation;
using GroceryListHelper.Shared.Models.Authentication;

namespace GroceryListHelper.Server.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequestModel>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Password).NotEmpty().Length(6, 30).Must(x => x.Any(char.IsDigit)).WithMessage("Password must contain at least one digit.");
        RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Confirm password should be the same as password.");
    }
}

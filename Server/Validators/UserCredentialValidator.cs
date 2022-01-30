using FluentValidation;
using GroceryListHelper.Shared.Models.Authentication;

namespace GroceryListHelper.Server.Validators;

public class UserCredentialValidator : AbstractValidator<UserCredentialsModel>
{
    public UserCredentialValidator()
    {
        RuleFor(x => x.Password).NotEmpty().Length(6, 30).Must(x => x.Any(char.IsDigit)).WithMessage("Password must contain at least one digit.");
    }
}

using FluentValidation;

namespace GroceryListHelper.Shared.Validators
{
    public class ChangeEmailRequestValidator : AbstractValidator<ChangeEmailRequest>
    {
        public ChangeEmailRequestValidator()
        {
            RuleFor(x => x.NewEmail).EmailAddress();
        }
    }
}

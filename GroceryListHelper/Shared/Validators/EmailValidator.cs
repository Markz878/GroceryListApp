using FluentValidation;

namespace GroceryListHelper.Shared.Validators
{
    public class EmailValidator : AbstractValidator<EmailModel>
    {
        public EmailValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage(x=>x.Email + " is not a valid email");
        }
    }
}

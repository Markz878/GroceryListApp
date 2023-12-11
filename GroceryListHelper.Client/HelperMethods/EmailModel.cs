namespace GroceryListHelper.Client.HelperMethods;

public sealed class EmailModel
{
    public string Email { get; set; } = string.Empty;
}

public sealed class EmailModelValidator : AbstractValidator<EmailModel>
{
    public EmailModelValidator()
    {
        RuleFor(x => x.Email).NotEmpty().MaximumLength(100).EmailAddress();
    }
}

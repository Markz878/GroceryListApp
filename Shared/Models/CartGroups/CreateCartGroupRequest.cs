using FluentValidation;

namespace GroceryListHelper.Shared.Models.CartGroups;

public class CreateCartGroupRequest
{
    public string Name { get; set; } = "";
    public HashSet<string> OtherUsers { get; init; } = new();
}

public class CreateCartGroupRequestValidator : AbstractValidator<CreateCartGroupRequest>
{
    public CreateCartGroupRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(30);
        RuleFor(x => x.OtherUsers)
            .Must(x => x.Count is > 0 and < 10).WithMessage("Group size must be between 2 and 10");
        RuleForEach(x => x.OtherUsers).EmailAddress();
    }
}

using FluentValidation;

namespace GroceryListHelper.Shared.Models.CartGroups;
public class UpdateCartGroupNameRequest
{
    public string Name { get; set; } = "";
}

public class UpdateCartGroupNameRequestValidator : AbstractValidator<UpdateCartGroupNameRequest>
{
    public UpdateCartGroupNameRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(30);
    }
}

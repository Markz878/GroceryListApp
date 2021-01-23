using FluentValidation;
using GroceryListHelper.Server.Data;
using GroceryListHelper.Shared;
using System.Linq;

namespace GroceryListHelper.Server.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequestModel>
    {
        private readonly GroceryStoreDbContext db;

        public RegisterRequestValidator(GroceryStoreDbContext db)
        {
            this.db = db;
        }

        public RegisterRequestValidator()
        {
            RuleFor(x => x.Email).EmailAddress().Must(IsUnique).WithMessage("Email is already registered.");
            RuleFor(x => x.Password).Cascade(CascadeMode.Stop).NotEmpty().Length(6, 30).Must(x => x.Any(char.IsDigit)).WithMessage("Password must contain at least one digit");
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Confirm password should be the same as password.");
        }

        private bool IsUnique(string email)
        {
            return db.Users.All(x => !x.Email.Equals(email));
        }
    }
}

using FluentValidation;
using GroceryListHelper.DataAccess.Models;
using GroceryListHelper.DataAccess.Repositories;
using GroceryListHelper.Shared;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GroceryListHelper.Server.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequestModel>
    {
        private readonly UserRepository db;

        public RegisterRequestValidator(UserRepository db)
        {
            this.db = db;
        }

        public RegisterRequestValidator()
        {
            RuleFor(x => x.Email).EmailAddress().MustAsync(IsUnique).WithMessage("Email is already registered.");
            RuleFor(x => x.Password).Cascade(CascadeMode.Stop).NotEmpty().Length(6, 30).Must(x => x.Any(char.IsDigit)).WithMessage("Password must contain at least one digit");
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Confirm password should be the same as password.");
        }

        private async Task<bool> IsUnique(string email, CancellationToken cancellationToken)
        {
            UserDbModel user = await db.GetUserFromEmail(email);
            return user == null;
        }
    }
}

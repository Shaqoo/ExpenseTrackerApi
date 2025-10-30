using ExpenseTracker.DTOS.User;
using FluentValidation;

namespace ExpenseTracker.Validators
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(150);
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
            RuleFor(x => x.DefaultCurrency).MaximumLength(5);
        }
    }
}
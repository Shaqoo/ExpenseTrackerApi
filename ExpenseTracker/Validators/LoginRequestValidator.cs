using ExpenseTracker.DTOS.Auth;
using FluentValidation;

namespace ExpenseTracker.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(150);
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        }
    }
}
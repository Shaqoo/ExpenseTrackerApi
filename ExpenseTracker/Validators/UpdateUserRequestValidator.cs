using ExpenseTracker.DTOS.User;
using FluentValidation;

namespace ExpenseTracker.Validators
{
    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidator()
        {
            RuleFor(x => x.FullName).MaximumLength(100).When(x => !string.IsNullOrEmpty(x.FullName));
            RuleFor(x => x.DefaultCurrency).MaximumLength(5).When(x => !string.IsNullOrEmpty(x.DefaultCurrency));
        }
    }
}
using ExpenseTracker.DTOS.Expense;
using FluentValidation;

namespace ExpenseTracker.Validators
{
    public class UpdateExpenseRequestValidator : AbstractValidator<UpdateExpenseRequest>
    {
        public UpdateExpenseRequestValidator()
        {
            RuleFor(x => x.Title).MaximumLength(150).When(x => !string.IsNullOrEmpty(x.Title));
            RuleFor(x => x.Amount).GreaterThan(0).When(x => x.Amount.HasValue);
            RuleFor(x => x.Notes).MaximumLength(500).When(x => x.Notes != null);
        }
    }
}
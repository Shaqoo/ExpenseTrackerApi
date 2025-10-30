using ExpenseTracker.DTOS.Expense;
using FluentValidation;

namespace ExpenseTracker.Validators
{
    public class CreateExpenseRequestValidator : AbstractValidator<CreateExpenseRequest>
    {
        public CreateExpenseRequestValidator()
        {
            RuleFor(x => x.CategoryId).NotEmpty();
            RuleFor(x => x.Title).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Amount).NotEmpty().GreaterThan(0);
            RuleFor(x => x.Currency).NotEmpty().MaximumLength(5);
            RuleFor(x => x.Notes).MaximumLength(500);
        }
    }
}
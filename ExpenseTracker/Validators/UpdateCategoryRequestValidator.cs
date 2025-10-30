using ExpenseTracker.DTOS.Category;
using FluentValidation;

namespace ExpenseTracker.Validators
{
    public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
    {
        public UpdateCategoryRequestValidator()
        {
            RuleFor(x => x.Name).MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Name));
            RuleFor(x => x.Description).MaximumLength(255).When(x => x.Description != null);
        }
    }
}
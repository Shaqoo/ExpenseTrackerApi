using ExpenseTracker.DTOS.Common;
using ExpenseTracker.DTOS.Expense;

namespace ExpenseTracker.Services.Interfaces
{
    public interface IExpenseService
    {
        Task<ApiResponse<ExpenseDto>> CreateExpenseAsync(CreateExpenseRequest request);
        Task<ApiResponse<IEnumerable<ExpenseDto>>> GetExpensesForUserAsync();
        Task<ApiResponse<ExpenseDto>> UpdateExpenseAsync(Guid expenseId, UpdateExpenseRequest request);
        Task<ApiResponse<bool>> DeleteExpenseAsync(Guid expenseId);
    }
}
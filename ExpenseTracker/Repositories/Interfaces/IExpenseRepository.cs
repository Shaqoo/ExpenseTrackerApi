using ExpenseTracker.Entities;

namespace ExpenseTracker.Repositories.Interfaces
{
    public interface IExpenseRepository
    {
        Task AddAsync(Expense expense);
        Task<Expense?> GetByIdAsync(Guid id);
        Task<IEnumerable<Expense>> GetByUserIdAsync(Guid userId);
        void Update(Expense expense);
        void Remove(Expense expense);
    }
}
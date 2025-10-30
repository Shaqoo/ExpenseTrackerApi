using ExpenseTracker.Entities;
using ExpenseTracker.ExDbContext;
using ExpenseTracker.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Repositories.Implementations
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly ExpenseDbContext _dbContext;

        public ExpenseRepository(ExpenseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Expense expense)
        {
            await _dbContext.Expenses.AddAsync(expense);
        }

        public Task<Expense?> GetByIdAsync(Guid id)
        {
            return _dbContext.Expenses.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Expense>> GetByUserIdAsync(Guid userId)
        {
            return await _dbContext.Expenses.AsNoTracking().Where(e => e.UserId == userId).ToListAsync();
        }

        public void Remove(Expense expense)
        {
            _dbContext.Expenses.Remove(expense);
        }

        public void Update(Expense expense)
        {
            _dbContext.Expenses.Update(expense);
        }
    }
}
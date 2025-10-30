using ExpenseTracker.Entities;
using ExpenseTracker.ExDbContext;
using ExpenseTracker.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Repositories.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ExpenseDbContext _dbContext;

        public CategoryRepository(ExpenseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Category category)
        {
            await _dbContext.Categories.AddAsync(category);
        }

        public async Task DeleteAsync(Category category)
        {
            _dbContext.Categories.Remove(category);
            await Task.CompletedTask;
        }

        public async Task<Category?> GetByIdAndUserIdAsync(Guid id, Guid userId)
        {
            return await _dbContext.Categories
            .AsNoTracking()
            .Where(c => c.Id == id && c.UserId == userId).FirstOrDefaultAsync();
        }

        public Task<Category?> GetByIdAsync(Guid id)
        {
            return _dbContext.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        }

        public Task<Category?> GetByNameAsync(string name, Guid userId)
        {
            return _dbContext.Categories.AsNoTracking().Where(c => c.Name == name && c.UserId == userId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Category>> GetByUserIdAsync(Guid userId)
        {
            return await _dbContext.Categories.AsNoTracking().Where(c => c.UserId == userId).ToListAsync();
        }
        public async Task UpdateAsync(Category category)
        {
            _dbContext.Categories.Update(category);
            await Task.CompletedTask;
        }
    }
}
using ExpenseTracker.Entities;

namespace ExpenseTracker.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task AddAsync(Category category);
        Task<Category?> GetByIdAsync(Guid id);
        Task<IEnumerable<Category>> GetByUserIdAsync(Guid userId);
        Task<Category?> GetByIdAndUserIdAsync(Guid id, Guid userId);
        Task<Category?> GetByNameAsync(string name, Guid userId);
        Task UpdateAsync(Category category);
        Task DeleteAsync(Category category);
    }
}
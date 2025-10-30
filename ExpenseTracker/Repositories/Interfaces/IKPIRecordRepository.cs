using ExpenseTracker.Entities;

namespace ExpenseTracker.Repositories.Interfaces
{
    public interface IKPIRecordRepository
    {
        Task AddAsync(KPIRecord kpiRecord);
        Task<IEnumerable<KPIRecord>> GetByUserIdAsync(Guid userId);
    }
}
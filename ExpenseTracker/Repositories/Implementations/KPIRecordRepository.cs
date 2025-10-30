using ExpenseTracker.Entities;
using ExpenseTracker.ExDbContext;
using ExpenseTracker.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Repositories.Implementations
{
    public class KPIRecordRepository : IKPIRecordRepository
    {
        private readonly ExpenseDbContext _dbContext;

        public KPIRecordRepository(ExpenseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(KPIRecord kpiRecord)
        {
            await _dbContext.KPIRecords.AddAsync(kpiRecord);
        }

        public async Task<IEnumerable<KPIRecord>> GetByUserIdAsync(Guid userId)
        {
            return await _dbContext.KPIRecords.AsNoTracking().Where(k => k.UserId == userId).ToListAsync();
        }
    }
}
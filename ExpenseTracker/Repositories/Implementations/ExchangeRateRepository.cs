using ExpenseTracker.Entities;
using ExpenseTracker.ExDbContext;
using ExpenseTracker.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Repositories.Implementations
{
    public class ExchangeRateRepository : IExchangeRateRepository
    {
        private readonly ExpenseDbContext _dbContext;

        public ExchangeRateRepository(ExpenseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddRangeAsync(IEnumerable<ExchangeRate> rates)
        {
            await _dbContext.ExchangeRates.AddRangeAsync(rates);
        }

        public Task<ExchangeRate?> GetByCurrencyAsync(string baseCurrency, string targetCurrency)
        {
            return _dbContext.ExchangeRates.AsNoTracking().FirstOrDefaultAsync(r => r.BaseCurrency == baseCurrency && r.TargetCurrency == targetCurrency);
        }

        public async Task BulkUpsertAsync(IEnumerable<ExchangeRate> rates)
        {
            foreach (var rate in rates)
            {
                var existing = await _dbContext.ExchangeRates
                    .FirstOrDefaultAsync(x => x.BaseCurrency == rate.BaseCurrency && x.TargetCurrency == rate.TargetCurrency);

                if (existing != null)
                {
                    existing.Rate = rate.Rate;
                    existing.RetrievedAt = rate.RetrievedAt;
                }
                else
                {
                    _dbContext.ExchangeRates.Add(rate);
                }
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
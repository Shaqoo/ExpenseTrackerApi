using ExpenseTracker.Entities;

namespace ExpenseTracker.Repositories.Interfaces
{
    public interface IExchangeRateRepository
    {
        Task AddRangeAsync(IEnumerable<ExchangeRate> rates);
        Task<ExchangeRate?> GetByCurrencyAsync(string baseCurrency, string targetCurrency);
        Task BulkUpsertAsync(IEnumerable<ExchangeRate> rates);
    }
}
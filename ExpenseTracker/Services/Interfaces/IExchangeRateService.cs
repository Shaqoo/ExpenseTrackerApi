using ExpenseTracker.DTOS.Common;
using ExpenseTracker.DTOS.ExchangeRate;

namespace ExpenseTracker.Services.Interfaces
{
    public interface IExchangeRateService
    {
        Task<ApiResponse<decimal>> ConvertAmountAsync(decimal amount, string fromCurrency, string toCurrency);
        Task<ApiResponse<ExchangeRateDto>> GetExchangeRateAsync(string baseCurrency, string targetCurrency);
    }
}
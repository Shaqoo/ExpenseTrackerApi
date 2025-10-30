using ExpenseTracker.DTOS.Common;
using ExpenseTracker.DTOS.ExchangeRate;
using ExpenseTracker.Repositories.Interfaces;
using ExpenseTracker.Services.Helpers;
using ExpenseTracker.Services.Interfaces;

namespace ExpenseTracker.Services.Implementations
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly IExchangeRateRepository _exchangeRateRepository;
        private readonly ILogger<ExchangeRateService> _logger;

        public ExchangeRateService(IExchangeRateRepository exchangeRateRepository, ILogger<ExchangeRateService> logger)
        {
            _exchangeRateRepository = exchangeRateRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<decimal>> ConvertAmountAsync(decimal amount, string fromCurrency, string toCurrency)
        {
            try
            {
                if (string.Equals(fromCurrency, toCurrency, StringComparison.OrdinalIgnoreCase))
                {
                    return ApiResponse<decimal>.SuccessResponse(amount, "Conversion not needed for the same currency.");
                }

                const string baseCurrency = "USD";

                var fromRate = fromCurrency == baseCurrency ? 1 : (await _exchangeRateRepository.GetByCurrencyAsync(baseCurrency, fromCurrency))?.Rate;
                var toRate = toCurrency == baseCurrency ? 1 : (await _exchangeRateRepository.GetByCurrencyAsync(baseCurrency, toCurrency))?.Rate;

                if (fromRate == null)
                {
                    _logger.LogWarning("Exchange rate not found for currency: {Currency}", fromCurrency);
                    return ApiResponse<decimal>.FailureResponse($"Exchange rate not found for currency: {fromCurrency}.");
                }

                if (toRate == null)
                {
                    _logger.LogWarning("Exchange rate not found for currency: {Currency}", toCurrency);
                    return ApiResponse<decimal>.FailureResponse($"Exchange rate not found for currency: {toCurrency}.");
                }

                var amountInBase = amount / fromRate.Value;
                var convertedAmount = amountInBase * toRate.Value;

                return ApiResponse<decimal>.SuccessResponse(convertedAmount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during currency conversion from {FromCurrency} to {ToCurrency}.", fromCurrency, toCurrency);
                return ApiResponse<decimal>.FailureResponse(StaticMessages.DefaultErrorMessage);
            }
        }

        public async Task<ApiResponse<ExchangeRateDto>> GetExchangeRateAsync(string baseCurrency, string targetCurrency)
        {
            try
            {
                var exchangeRate = await _exchangeRateRepository.GetByCurrencyAsync(baseCurrency, targetCurrency);

                if (exchangeRate == null)
                {
                    _logger.LogWarning("Exchange rate not found for {BaseCurrency} to {TargetCurrency}.", baseCurrency, targetCurrency);
                    return ApiResponse<ExchangeRateDto>.FailureResponse($"Exchange rate not found from {baseCurrency} to {targetCurrency}.");
                }

                var exchangeRateDto = new ExchangeRateDto
                {
                    BaseCurrency = exchangeRate.BaseCurrency,
                    TargetCurrency = exchangeRate.TargetCurrency,
                    Rate = exchangeRate.Rate,
                    RetrievedAt = exchangeRate.RetrievedAt
                };

                return ApiResponse<ExchangeRateDto>.SuccessResponse(exchangeRateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the exchange rate for {BaseCurrency} to {TargetCurrency}.", baseCurrency, targetCurrency);
                return ApiResponse<ExchangeRateDto>.FailureResponse(StaticMessages.DefaultErrorMessage);
            }
        }
    }
}
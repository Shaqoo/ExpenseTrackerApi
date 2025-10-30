using System.Text.Json;
using ExpenseTracker.DTOS;
using ExpenseTracker.Entities;
using ExpenseTracker.Repositories.Interfaces;

namespace ExpenseTracker.ExternalServices
{
    public class ExchangeRateUpdaterService : IExchangeRateUpdaterService
    {
        private readonly HttpClient _httpClient;
        private readonly IExchangeRateRepository _exchangeRateRepository;
        private readonly ILogger<ExchangeRateUpdaterService> _logger;

        public ExchangeRateUpdaterService(HttpClient httpClient, IExchangeRateRepository exchangeRateRepository, ILogger<ExchangeRateUpdaterService> logger)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
           // _httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
            _exchangeRateRepository = exchangeRateRepository;
            _logger = logger;
        }

        public async Task UpdateRateAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://open.er-api.com/v6/latest/USD");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to fetch exchange rates. Status Code: {StatusCode}", response.StatusCode);
                    return;
                }

                var responeBody = await response.Content.ReadAsStringAsync();

                var exchangeRateResponse = JsonSerializer.Deserialize<ExchangeRateResponse>(
                    responeBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (exchangeRateResponse == null || exchangeRateResponse.Rates == null)
                {
                    _logger.LogError("Exchange rate response deserialization failed.");
                    return;
                }

                _logger.LogInformation("Fetched {Count} exchange rates for base {BaseCurrency}.",
                    exchangeRateResponse.Rates.Count, exchangeRateResponse.BaseCurrency);

                var now = DateTime.UtcNow;
                var rates = exchangeRateResponse.Rates.Select(a => new ExchangeRate
                {
                    BaseCurrency = exchangeRateResponse.BaseCurrency,
                    TargetCurrency = a.Key,
                    Rate = a.Value,
                    RetrievedAt = now,
                    Source = "OpenExchangeRates"
                });

                await _exchangeRateRepository.BulkUpsertAsync(rates);
                _logger.LogInformation("Exchange rates updated successfully at {Time}.", now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating exchange rates.");
            }
        }
    }
}
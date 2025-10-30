using System.Text.Json.Serialization;

namespace ExpenseTracker.DTOS
{
    public class ExchangeRateResponse
    {
        [JsonPropertyName("result")]
        public string Status { get; set; } = default!;
        [JsonPropertyName("base_code")]
        public string BaseCurrency { get; set; } = default!;
        [JsonPropertyName("rates")]
        public Dictionary<string, decimal> Rates { get; set; } = new Dictionary<string, decimal>();
    }
}
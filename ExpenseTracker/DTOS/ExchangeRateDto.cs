namespace ExpenseTracker.DTOS.ExchangeRate
{
    public class ExchangeRateDto
    {
        public string BaseCurrency { get; set; } = default!;
        public string TargetCurrency { get; set; } = default!;
        public decimal Rate { get; set; }
        public DateTime RetrievedAt { get; set; }
    }
}
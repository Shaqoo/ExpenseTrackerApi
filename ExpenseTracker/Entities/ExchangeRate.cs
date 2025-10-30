namespace ExpenseTracker.Entities
{
    public class ExchangeRate
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string BaseCurrency { get; set; } = default!;
        public string TargetCurrency { get; set; } = default!;
        public decimal Rate { get; set; }
        public string Source { get; set; } = default!;
        public DateTime RetrievedAt { get; set; } = DateTime.UtcNow;
    }

}
namespace ExpenseTracker.Entities
{
    public class KPIRecord
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string MetricName { get; set; } = default!;
        public decimal Value { get; set; }
        public string Currency { get; set; } = "USD";
        public DateTime Period { get; set; } = DateTime.UtcNow;
    }

}
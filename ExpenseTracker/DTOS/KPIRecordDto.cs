namespace ExpenseTracker.DTOS.KPI
{
    public class KPIRecordDto
    {
        public Guid Id { get; set; }
        public string MetricName { get; set; } = default!;
        public decimal Value { get; set; }
        public string Currency { get; set; } = default!;
        public DateTime Period { get; set; }
    }
}
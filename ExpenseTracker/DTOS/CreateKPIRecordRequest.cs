namespace ExpenseTracker.DTOS.KPI
{
    public class CreateKPIRecordRequest
    {
        public string MetricName { get; set; } = default!;
        public decimal Value { get; set; }
        public string? Currency { get; set; }
    }
}
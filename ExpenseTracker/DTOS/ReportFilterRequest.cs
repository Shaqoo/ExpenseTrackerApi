namespace ExpenseTracker.DTOS.Report
{
    public class ReportFilterRequest
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
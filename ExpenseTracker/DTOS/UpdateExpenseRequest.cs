using ExpenseTracker.Enums;

namespace ExpenseTracker.DTOS.Expense
{
    public class UpdateExpenseRequest
    {
        public Guid? CategoryId { get; set; }
        public string? Title { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? Date { get; set; }
        public string? Notes { get; set; }
    }
}
using ExpenseTracker.Enums;

namespace ExpenseTracker.DTOS.Expense
{
    public class CreateExpenseRequest
    {
        public Guid CategoryId { get; set; }
        public string Title { get; set; } = default!;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public TransactionType Type { get; set; } = TransactionType.Expense;
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string? Notes { get; set; }
    }
}
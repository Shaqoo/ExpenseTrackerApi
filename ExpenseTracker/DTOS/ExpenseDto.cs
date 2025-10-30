using ExpenseTracker.Enums;

namespace ExpenseTracker.DTOS.Expense
{
    public class ExpenseDto
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string Title { get; set; } = default!;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = default!;
        public decimal ConvertedAmountUSD { get; set; }
        public TransactionType Type { get; set; }
        public DateTime Date { get; set; }
        public string? Notes { get; set; }
    }
}
using System.Transactions;
using ExpenseTracker.Enums;

namespace ExpenseTracker.Entities
{
    public class Expense
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public User User { get; set; } = default!;
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = default!;
        public string Title { get; set; } = default!;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public decimal ConvertedAmountUSD { get; set; }
        public TransactionType Type { get; set; } = TransactionType.Expense;
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string Notes { get; set; } = default!;
        public bool IsRecurring { get; set; }
        public byte[] RowVersion { get; set; } = BitConverter.GetBytes(DateTime.UtcNow.Ticks);
    }
}

using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string FullName { get; set; } = default!;
        public string DefaultCurrency { get; set; } = "USD";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public byte[] RowVersion { get; set; } = BitConverter.GetBytes(DateTime.UtcNow.Ticks);
    }

}
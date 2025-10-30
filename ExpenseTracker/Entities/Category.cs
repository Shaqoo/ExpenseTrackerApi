namespace ExpenseTracker.Entities
{
    public class Category
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public User User { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }

}
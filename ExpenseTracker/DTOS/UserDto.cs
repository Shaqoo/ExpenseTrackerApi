namespace ExpenseTracker.DTOS.User
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string DefaultCurrency { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
    }
}
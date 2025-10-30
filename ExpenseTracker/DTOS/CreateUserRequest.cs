namespace ExpenseTracker.DTOS.User
{
    public class CreateUserRequest
    {
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string ConfirmPassword { get; set; } = default!;
        public string? DefaultCurrency { get; set; } = "USD";
    }
}